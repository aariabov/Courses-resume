import * as monaco from "monaco-editor";
import { apiClient } from "../api/ApiClient";

let providerRegistered = false;

export function setupMonacoProviders() {
  if (providerRegistered) {
    return;
  }

  monaco.languages.registerCompletionItemProvider("csharp", {
    triggerCharacters: ["."],
    provideCompletionItems: async (model, position) => {
      const code = model.getValue();
      const offset = model.getOffsetAt(position);
      const response = await apiClient.api.clientCodeGetCompletion({ code: code, position: offset });
      const wordInfo = model.getWordUntilPosition(position);
      const range = new monaco.Range(
        position.lineNumber,
        wordInfo.startColumn,
        position.lineNumber,
        wordInfo.endColumn,
      );
      // Преобразование в CompletionItem[]
      const suggestions: monaco.languages.CompletionItem[] = response.data.data.map((item) => ({
        label: item.displayText,
        kind: mapRoslynTagToMonacoKind(item.tags?.[0] ?? ""),
        insertText: item.displayText,
        range: range,
      }));
      return { suggestions };
    },
  });

  providerRegistered = true;
}

function mapRoslynTagToMonacoKind(tag: string): monaco.languages.CompletionItemKind {
  switch (tag) {
    case "Method":
      return monaco.languages.CompletionItemKind.Method;
    case "Function":
      return monaco.languages.CompletionItemKind.Function;
    case "Constructor":
      return monaco.languages.CompletionItemKind.Constructor;
    case "Field":
      return monaco.languages.CompletionItemKind.Field;
    case "Variable":
      return monaco.languages.CompletionItemKind.Variable;
    case "Class":
      return monaco.languages.CompletionItemKind.Class;
    case "Struct":
      return monaco.languages.CompletionItemKind.Struct;
    case "Interface":
      return monaco.languages.CompletionItemKind.Interface;
    case "Enum":
      return monaco.languages.CompletionItemKind.Enum;
    case "Property":
      return monaco.languages.CompletionItemKind.Property;
    case "Event":
      return monaco.languages.CompletionItemKind.Event;
    case "Constant":
      return monaco.languages.CompletionItemKind.Constant;
    case "EnumMember":
      return monaco.languages.CompletionItemKind.EnumMember;
    case "Namespace":
      return monaco.languages.CompletionItemKind.Module;
    case "Operator":
      return monaco.languages.CompletionItemKind.Operator;
    case "Parameter":
      return monaco.languages.CompletionItemKind.Variable;
    default:
      return monaco.languages.CompletionItemKind.Text;
  }
}
