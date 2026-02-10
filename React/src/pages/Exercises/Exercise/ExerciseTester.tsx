import { Flex, Modal, Splitter, theme, Tooltip } from "antd";
import React, { useRef, useState } from "react";
import { DiagnosticMsg, DiagnosticSeverity, ExerciseDto } from "../../../api/Api";
import { apiClient } from "../../../api/ApiClient";
import { useParams } from "react-router-dom";
import * as monaco from "monaco-editor";
import { Editor } from "@monaco-editor/react";
import { editor as monacoTypes, MarkerSeverity } from "monaco-editor";
import { LoadingOutlined, PlayCircleOutlined, UndoOutlined } from "@ant-design/icons";
import { css } from "@emotion/react";
import { observer } from "mobx-react-lite";
import Terminal from "../../../components/Terminal";

type ExerciseTesterProps = {
  exercise: ExerciseDto;
  onAccepted: () => void;
};

const ExerciseTester: React.FC<ExerciseTesterProps> = observer(({ exercise, onAccepted }) => {
  const { token } = theme.useToken();
  const [isDark] = useState<boolean>(false);
  const [code, setCode] = useState(exercise.template);
  const [isShowResetBtn, setIsShowResetBtn] = useState<boolean>(false);
  const debounceTimeout = useRef<ReturnType<typeof setTimeout> | null>(null);
  const fullScreenEditorRef = useRef<monacoTypes.IStandaloneCodeEditor | null>(null);
  const [showResult, setShowResult] = useState<boolean>(false);
  const [showLoading, setShowLoading] = useState<boolean>(false);
  const [result, setResult] = useState<string>("");
  const fullScreenTerminalInputRef = useRef<HTMLInputElement>(null);
  // params are not used here but keep for potential future use
  useParams();

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
    const model = fullScreenEditorRef.current?.getModel();

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

  const cleanInputAndRun = (currentCode: string) => {
    setCode(currentCode);
    setShowResult(false);
    run(currentCode);
  };

  async function run(codeValue: string) {
    if (!codeValue) {
      return;
    }

    if (fullScreenEditorRef.current) {
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
          const errorsStr = execResult.data.data.testErrors
            .map((r: any) => exercise.functionName + "(" + r.parameters.join(", ") + ") " + r.error)
            .join("\r\n");

          setResult(`Неправильно:\n${errorsStr}`);
        } else {
          setResult(`Успешно!`);
          onAccepted();
        }
      } catch (error) {
        console.error("Validation failed:", error);
      } finally {
        setShowResult(true);
        setShowLoading(false);
      }
    }
  }

  return (
    <>
      <Flex
        vertical={false}
        gap="1em"
        style={{
          padding: "0.5em 1em",
          borderBottom: "1px solid #dcdcdb",
        }}
      >
        {showLoading ? (
          <LoadingOutlined css={btnClass} />
        ) : (
          <Tooltip title="Ctrl + Enter">
            <PlayCircleOutlined onClick={() => cleanInputAndRun(code)} css={btnClass} />
          </Tooltip>
        )}
        {isShowResetBtn && <UndoOutlined onClick={showResetConfirm} css={btnClass} />}
      </Flex>
      <Splitter layout="vertical">
        <Splitter.Panel min="20%">
          <Editor
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
              scrollbar: {
                vertical: "auto",
                horizontal: "auto",
              },
            }}
          />
        </Splitter.Panel>
        <Splitter.Panel min="20%">
          {showResult && (
            <div
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
          )}
        </Splitter.Panel>
      </Splitter>
    </>
  );
});

export default ExerciseTester;
