import {
  FullscreenExitOutlined,
  FullscreenOutlined,
  LoadingOutlined,
  PlayCircleOutlined,
  UndoOutlined,
} from "@ant-design/icons";
import { css } from "@emotion/react";
import { Editor } from "@monaco-editor/react";
import { Flex, Grid, Modal, theme, Tooltip, Typography } from "antd";
import { observer } from "mobx-react-lite";
import * as monaco from "monaco-editor"; // глобальная библиотека
import { MarkerSeverity, editor as monacoTypes } from "monaco-editor";
import React, { useEffect, useRef, useState } from "react";
import { DiagnosticMsg, DiagnosticSeverity, ExerciseDto } from "../api/Api";
import { apiClient } from "../api/ApiClient";
import Terminal from "./Terminal";

type FunctionTesterProps = {
  exercise: ExerciseDto;
  idx: number;
};

const { Text } = Typography;
const { useBreakpoint } = Grid;

const FunctionTester: React.FC<FunctionTesterProps> = observer(({ exercise, idx }) => {
  const { token } = theme.useToken();
  const [isDark] = useState<boolean>(false);
  const [showResult, setShowResult] = useState<boolean>(false);
  const [showLoading, setShowLoading] = useState<boolean>(false);
  const [code, setCode] = useState(exercise.template);
  const [result, setResult] = useState<string>("");
  const containerRef = useRef(null);
  const editorRef = useRef<monacoTypes.IStandaloneCodeEditor | null>(null);
  const fullScreenEditorRef = useRef<monacoTypes.IStandaloneCodeEditor | null>(null);
  const [editorHeight, setEditorHeight] = useState(0);
  const [isShowResetBtn, setIsShowResetBtn] = useState<boolean>(false);
  const [canRun, setCanRun] = useState<boolean>(true);
  const [isFullScreenMode, setIsFullScreenMode] = useState<boolean>(false);
  const [isFullScreenWasOpened, setIsFullScreenWasOpened] = useState<boolean>(false);
  const [terminalHeight, setTerminalHeight] = useState(0);
  const debounceTimeout = useRef<ReturnType<typeof setTimeout> | null>(null);
  const terminalInputRef = useRef<HTMLInputElement>(null);
  const fullScreenTerminalInputRef = useRef<HTMLInputElement>(null);
  const terminalContainerRef = useRef<HTMLInputElement>(null);
  const fullScreenTerminalContainerRef = useRef<HTMLInputElement>(null);
  const screens = useBreakpoint();

  // Подсчёт высоты при монтировании
  useEffect(() => {
    const lineCount = exercise.template.split("\n").length;
    const lineHeight = 19; // высота строки в monaco editor
    const height = lineCount * lineHeight;
    const minHeight = canRun ? 90 : lineHeight; // из-за кнопок, но можно их перенести вверх
    const maxHeight = document.documentElement.clientHeight; // весь экран

    if (height <= minHeight) {
      setEditorHeight(minHeight);
    } else if (height >= maxHeight) {
      setEditorHeight(maxHeight);
    } else {
      setEditorHeight(height);
    }
  }, [exercise.template]);

  // Подсчёт высоты терминала, в зависимости от вывода
  useEffect(() => {
    const lineHeight = 19;
    const maxHeight = document.documentElement.clientHeight / 2; // пол экрана
    const lines = result.split(/\r?\n/);
    const height = (lines.length + 1) * lineHeight;
    if (height >= maxHeight) {
      setTerminalHeight(maxHeight);
    } else {
      setTerminalHeight(height);
    }
  }, [result]);

  const changeCode = async (value?: string) => {
    setCode(value ?? "");

    const codeWasChanged: boolean = value !== exercise.template;
    setIsShowResetBtn(codeWasChanged);

    if (value) {
      if (debounceTimeout.current) {
        clearTimeout(debounceTimeout.current);
      }

      debounceTimeout.current = setTimeout(() => {
        validateCode(value);
      }, 1000);
    }
  };

  const getSeverity = (severity: DiagnosticSeverity): MarkerSeverity => {
    switch (severity) {
      case "Error":
        return MarkerSeverity.Error;
      case "Warning":
        return MarkerSeverity.Warning;
      case "Info":
        return MarkerSeverity.Info;
      case "Hidden":
        return MarkerSeverity.Hint;
    }
  };

  const validateCode = async (value: string) => {
    const res = await apiClient.api.clientCodeAnalyze({ code: value });
    const errors = res.data.data;
    const model = (isFullScreenMode ? fullScreenEditorRef : editorRef).current?.getModel();

    if (!model) {
      return;
    }

    const markers: monacoTypes.IMarkerData[] = errors.map((e: DiagnosticMsg) => ({
      severity: getSeverity(e.severity),
      message: e.message,
      startLineNumber: e.startLine + 1,
      startColumn: e.startColumn + 1,
      endLineNumber: e.endLine + 1,
      endColumn: e.endColumn + 1,
    }));

    monaco.editor.setModelMarkers(model, "owner", markers);
  };

  const btnClass = css({
    fontSize: "2em",
    color: token.colorPrimary,
    ":hover": {
      color: `${token.colorPrimaryTextActive}`,
    },
  });

  const { confirm } = Modal;
  const showResetConfirm = () => {
    confirm({
      title: "Вы действительно хотите сбросить все изменения?",
      okText: "Сбросить",
      cancelText: "Отмена",
      async onOk() {
        setCode(exercise.template);
        setShowResult(false);
        setIsShowResetBtn(false);
        validateCode(exercise.template);
      },
    });
  };

  const exitFromFullScreenMode = () => {
    setIsFullScreenMode(false);
    if (editorRef.current && !showResult) {
      setFocusToEditor(editorRef.current);
    } else {
      fullScreenTerminalInputRef.current?.focus();
      fullScreenTerminalInputRef.current?.scrollIntoView();
    }
  };

  const setFocusToEditor = (editor: monacoTypes.IStandaloneCodeEditor) => {
    const model = editor.getModel();
    if (!model) {
      return;
    }

    const lastLine = model.getLineCount();
    const lastCol = model.getLineMaxColumn(lastLine);

    editor.focus(); // Установить фокус
    editor.setPosition({ lineNumber: lastLine, column: lastCol });
    editor.revealPositionInCenter({ lineNumber: lastLine, column: lastCol });
  };

  const cleanInputAndRun = (currentCode: string) => {
    setCode(currentCode);
    setShowResult(false);
    run(currentCode);
  };

  async function run(codeValue: string) {
    if (!codeValue) {
      return;
    }

    if (editorRef.current) {
      try {
        setShowLoading(true);
        setShowResult(false);
        const execResult = await apiClient.api.courseTest({
          exerciseId: exercise.id,
          code: codeValue,
        });

        if (execResult.data.data.status === "Error") {
          setResult(`Ошибка:\n${execResult.data.data.error}`);
        } else if (execResult.data.data.status === "Fail") {
          setResult(
            `Неправильно:\n${execResult.data.data.testErrors.map((r) => `${exercise.functionName}(${r.parameters.join(", ")}) ${r.error}`).join("\r\n")}`,
          );
        } else {
          setResult(`Успешно!`);
        }
      } catch (error) {
        console.log("Validation failed:", error);
      } finally {
        setShowResult(true);
        setShowLoading(false);
      }
    }
  }

  let isVertical = true;
  if (screens.xl) {
    isVertical = false;
  }

  return (
    <>
      <div
        style={{
          backgroundColor: isDark ? "#1e1e1e" : "#fffffe" /* цвет monaco editor */,
          border: "1px solid",
          borderColor: isDark ? "#1e1e1e" : "#dcdcdb",
        }}
      >
        <Flex vertical={false}>
          <div
            ref={containerRef}
            style={{
              flex: "1 1 auto",
              height: `${editorHeight}px`,
            }}
          >
            {!isFullScreenMode && (
              <Editor
                height="100%"
                theme={isDark ? "vs-dark" : "light"}
                defaultLanguage="csharp"
                value={code}
                onChange={changeCode}
                onMount={(editor) => {
                  editorRef.current = editor;

                  editor.addAction({
                    id: "run-code",
                    label: "Run Code",
                    keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyCode.Enter],
                    run: (ed) => {
                      const currentCode = ed.getValue();
                      cleanInputAndRun(currentCode);
                    },
                  });

                  if (isFullScreenWasOpened && !showResult) {
                    setFocusToEditor(editor);
                  } else {
                    terminalInputRef.current?.focus();
                    terminalInputRef.current?.scrollIntoView();
                  }
                  validateCode(editor.getValue());
                }}
                options={{
                  minimap: { enabled: false },
                  scrollBeyondLastLine: false,
                  renderLineHighlight: "none",
                  readOnly: !canRun,
                  scrollbar: {
                    vertical: "auto",
                    horizontal: "auto",
                  },
                }}
              />
            )}
          </div>
          {!isFullScreenMode && canRun && (
            <div
              style={{
                flex: "0 0 auto",
                padding: "5px",
                borderLeft: isDark ? "1px solid #393939" : "1px solid #dcdcdb",
              }}
            >
              <Flex vertical={true} gap="1em">
                {showLoading ? (
                  <LoadingOutlined css={btnClass} />
                ) : (
                  <Tooltip title="Ctrl + Enter">
                    <PlayCircleOutlined onClick={() => cleanInputAndRun(code)} css={btnClass} />
                  </Tooltip>
                )}
                <FullscreenOutlined
                  css={btnClass}
                  onClick={() => {
                    setIsFullScreenWasOpened(true);
                    setIsFullScreenMode(true);
                  }}
                />
                {isShowResetBtn && <UndoOutlined onClick={showResetConfirm} css={btnClass} />}
              </Flex>
            </div>
          )}
        </Flex>
        {showResult && (
          <div
            ref={terminalContainerRef}
            style={{
              height: `${terminalHeight}px`,
              overflow: "auto",
              borderTop: "1px solid",
              borderTopColor: isDark ? "#393939" : "#dcdcdb",
              scrollbarColor: isDark ? "#393939 #181818" : "#dcdcdb #ffffff",
              padding: "0.5em",
              boxSizing: "content-box",
            }}
          >
            <Terminal inputRef={terminalInputRef} addInput={(input: string) => {}} outputStr={result} readonly={true} />
          </div>
        )}

        {isFullScreenMode && (
          <Modal
            open={true}
            onCancel={exitFromFullScreenMode}
            footer={null}
            closable={true}
            width="100%"
            style={{ top: 0, padding: 0, width: "100vw", margin: 0 }}
            styles={{ content: { height: "100vh", width: "100vw" }, body: { height: "100%" } }}
          >
            <Flex vertical={isVertical} style={{ height: "100%" }}>
              <div
                style={{
                  flex: "1",
                  overflow: "hidden",
                }}
              >
                <Flex vertical={true} style={{ height: "100%" }}>
                  <div>
                    <Flex
                      vertical={false}
                      gap="1em"
                      style={{
                        justifyContent: "flex-end",
                        padding: "0 2em 0.5em 0",
                        borderBottom: "1px solid #dcdcdb",
                      }}
                    >
                      <div style={{ fontSize: "1.5em", marginRight: "auto", lineHeight: "2rem" }}>Devpull</div>
                      {isShowResetBtn && <UndoOutlined onClick={showResetConfirm} css={btnClass} />}
                      <FullscreenExitOutlined css={btnClass} onClick={exitFromFullScreenMode} />
                      {showLoading ? (
                        <LoadingOutlined css={btnClass} />
                      ) : (
                        <Tooltip title="Ctrl + Enter">
                          <PlayCircleOutlined onClick={() => cleanInputAndRun(code)} css={btnClass} />
                        </Tooltip>
                      )}
                    </Flex>
                  </div>
                  <div style={{ flex: 1 }}>
                    <Editor
                      height="100%"
                      theme={isDark ? "vs-dark" : "light"}
                      defaultLanguage="csharp"
                      value={code}
                      onChange={changeCode}
                      onMount={(editorInstance) => {
                        fullScreenEditorRef.current = editorInstance;

                        editorInstance.addAction({
                          id: "run-code",
                          label: "Run Code",
                          keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyCode.Enter],
                          run: (ed) => {
                            const currentCode = ed.getValue();
                            cleanInputAndRun(currentCode);
                          },
                        });

                        validateCode(editorInstance.getValue());
                        if (!showResult) {
                          setFocusToEditor(editorInstance);
                        } else {
                          fullScreenTerminalInputRef.current?.focus();
                          fullScreenTerminalInputRef.current?.scrollIntoView();
                        }
                      }}
                      options={{
                        minimap: { enabled: false },
                        scrollBeyondLastLine: false,
                        renderLineHighlight: "none",
                        readOnly: !canRun,
                        scrollbar: {
                          vertical: "auto",
                          horizontal: "auto",
                        },
                      }}
                    />
                  </div>
                </Flex>
              </div>
              <div
                style={{
                  flex: "1",
                  overflow: "hidden",
                  marginTop: "40px",
                  borderTop: "1px solid #dcdcdb",
                }}
              >
                <Flex vertical={true} style={{ height: "100%" }}>
                  <div
                    ref={fullScreenTerminalContainerRef}
                    style={{
                      flex: 1,
                      overflow: "auto",
                      scrollbarColor: isDark ? "#393939 #181818" : "#dcdcdb #ffffff",
                      padding: "0.5em",
                    }}
                  >
                    {showResult && (
                      <Terminal
                        inputRef={fullScreenTerminalInputRef}
                        addInput={(input: string) => {}}
                        outputStr={result}
                        readonly={true}
                      />
                    )}
                  </div>
                </Flex>
              </div>
            </Flex>
          </Modal>
        )}
      </div>
    </>
  );
});

export default FunctionTester;
