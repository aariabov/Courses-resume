import { Editor } from "@monaco-editor/react";
import React, { useEffect, useState } from "react";
import { Typography } from "antd";
import { FunctionTestingResultView } from "../../../api/Api";

type HistoryEditorProps = {
  code: string;
  functionName: string;
  resultView: FunctionTestingResultView;
};

const HistoryItem: React.FC<HistoryEditorProps> = ({ code, resultView, functionName }) => {
  const [editorHeight, setEditorHeight] = useState(0);
  const [result, setResult] = useState<string>("");

  useEffect(() => {
    const lineCount = code.split("\n").length;
    const lineHeight = 19; // height of a line in monaco editor
    const height = lineCount * lineHeight;
    const minHeight = 90;
    const maxHeight = document.documentElement.clientHeight; // full screen height

    if (height <= minHeight) {
      setEditorHeight(minHeight);
    } else if (height >= maxHeight) {
      setEditorHeight(maxHeight);
    } else {
      setEditorHeight(height);
    }
  }, [code]);

  useEffect(() => {
    if (resultView.status === "Error") {
      setResult(`Ошибка:\n${resultView.error}`);
    } else if (resultView.status === "Fail") {
      const errorsStr = resultView.testErrors
        .map((r: any) => functionName + "(" + r.parameters.join(", ") + ") " + r.error)
        .join("\r\n");

      setResult(`Неправильно:\n${errorsStr}`);
    } else {
      setResult(`Успешно!`);
    }
  }, [resultView]);

  return (
    <>
      <div style={{ marginBottom: 8, height: `${editorHeight}px` }}>
        <Editor
          height="100%"
          defaultLanguage="csharp"
          value={code}
          options={{ readOnly: true, minimap: { enabled: false }, scrollBeyondLastLine: false }}
        />
      </div>
      <Typography.Paragraph style={{ whiteSpace: "pre-wrap" }}>{result}</Typography.Paragraph>
    </>
  );
};

export default HistoryItem;
