import React, { ChangeEvent, KeyboardEvent, useEffect, useRef, useState } from "react";
import "./Terminal.css";

type TerminalProps = {
  outputStr?: string;
  addInput: (input: string) => void;
  readonly: boolean;
  inputRef: React.RefObject<HTMLInputElement>;
};

const Terminal: React.FC<TerminalProps> = ({ addInput, outputStr = "", readonly, inputRef }) => {
  const [input, setInput] = useState<string>("");
  const [prompt, setPrompt] = useState<string>("");
  const [output, setOutput] = useState<string>("");
  const terminalRef = useRef<HTMLInputElement>(null);
  const intervalRef = useRef<NodeJS.Timeout | null>(null);
  const [isReadonly, setIsReadonly] = useState<boolean>(false);
  const [isDark] = useState<boolean>(false);
  const backgroundColor = isDark ? "#181818" : "#ffffff";
  const color = isDark ? "#cccccc" : "#000000";

  useEffect(() => {
    inputRef.current?.focus();

    const lines = outputStr.split(/\r?\n/);
    if (lines.length > 0) {
      const lastLine = lines[lines.length - 1];
      if (lastLine) {
        setPrompt(lastLine);
        lines.pop();
        setOutput(lines.join("\r\n"));
      } else {
        setPrompt("");
        setOutput(outputStr);
      }
    }
  }, [inputRef, outputStr]);

  useEffect(() => {
    if (terminalRef.current) {
      terminalRef.current.scrollTop = terminalRef.current.scrollHeight;
    }

    if (intervalRef.current !== null) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
    setInput("");
  }, [output]);

  useEffect(() => {
    setIsReadonly(readonly || intervalRef.current !== null);
  }, [readonly, intervalRef]);

  useEffect(() => {
    return () => {
      if (intervalRef.current !== null) {
        clearInterval(intervalRef.current);
      }
    };
  }, []);

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    if (!isReadonly && e.key === "Enter") {
      addInput(input);
      setInput("");

      const spinner = ["", ".", "..", "..."];
      let i = 0;
      if (intervalRef.current !== null) {
        return;
      }

      intervalRef.current = setInterval(() => {
        i = i === spinner.length - 1 ? 0 : ++i;
        setInput(input + " " + spinner[i]);
      }, 300);
    }
  };

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    setInput(e.target.value);
  };

  const handleClick = () => {
    if (!readonly) {
      inputRef.current?.focus();
    }
  };

  return (
    <div
      ref={terminalRef}
      className="terminal"
      style={{
        backgroundColor: backgroundColor,
        color: color,
      }}
      onClick={handleClick}
    >
      <div className="terminal-output">{output}</div>
      {(!isReadonly || prompt) && <div className="terminal-line">
        <span className="terminal-prompt" style={{ color: color }}>
          {prompt}
        </span>
        {!isReadonly && <div className="terminal-input-wrapper">
          <input
            ref={inputRef}
            readOnly={isReadonly}
            className="terminal-input"
            style={{
              backgroundColor: backgroundColor,
              color: color,
            }}
            value={input}
            onChange={handleChange}
            onKeyDown={handleKeyDown}
          />
        </div>}
      </div>}
    </div>
  );
};

export default Terminal;
