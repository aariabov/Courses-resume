import { Alert, Grid, Image, Layout, theme, Typography } from "antd";
import Sider from "antd/es/layout/Sider";
import { Content } from "antd/es/layout/layout";
import Paragraph from "antd/es/typography/Paragraph";
import { Root } from "mdast";
import React from "react";
import ReactMarkdown from "react-markdown";
import { Light as SyntaxHighlighter } from "react-syntax-highlighter";
import csharp from "react-syntax-highlighter/dist/esm/languages/hljs/csharp";
import rehypeRaw from "rehype-raw";
import rehypeSlug from "rehype-slug";
import remarkGfm from "remark-gfm";
import remarkToc from "remark-toc"; // для внедрения в сам док
import { visit } from "unist-util-visit";
import { MARGIN } from "../constants";
import CodeExecutor from "./CodeExecutor";
import Toc from "./Toc";

const { Text } = Typography;
const { useBreakpoint } = Grid;

SyntaxHighlighter.registerLanguage("csharp", csharp);

type MarkdownProps = {
  markdown: string;
  showToc?: boolean;
};

const Md: React.FC<MarkdownProps> = ({ markdown, showToc = true }) => {
  const { token } = theme.useToken();
  const screens = useBreakpoint();

  if (screens.lg) {
    // для больших экранов tok будет в сайдбаре - вырезаем Содержание из md
    markdown = markdown
      .split("\n")
      .filter((line) => line !== "## Содержание\r")
      .join("\n");
  }

  return (
    <Layout>
      {showToc && (
        <Sider
          width="25%"
          style={{
            backgroundColor: token.Layout?.bodyBg,
            marginRight: MARGIN,
          }}
        >
          <Toc markdown={markdown} />
        </Sider>
      )}
      <Content>
        <ReactMarkdown
          remarkPlugins={[remarkGfm, remarkCodeMeta, [remarkToc, { heading: "Содержание", tight: true, maxDepth: 5 }]]}
          rehypePlugins={[rehypeRaw, rehypeSlug]}
          components={{
            b(props) {
              const { children } = props;
              return <Text strong>{children}</Text>;
            },
            p(props) {
              const { children } = props;
              return <Paragraph style={{}}>{children}</Paragraph>;
            },
            img(props) {
              const { src, alt } = props;
              return (
                <figure style={{ textAlign: "center" }}>
                  <Image src={src} alt={alt} style={{ maxWidth: "100%" }} />
                  <figcaption style={{ color: token.colorTextSecondary, fontSize: token.fontSizeSM }}>{alt}</figcaption>
                </figure>
              );
            },
            a(props) {
              const { href, children } = props;
              return href?.startsWith("http") ? (
                <a href={href} target="_blank" rel="noopener noreferrer">
                  {children}
                </a>
              ) : (
                <a href={href}>{children}</a>
              );
            },
            // для collapse, но не разобрался, как рендерить заголовок https://gist.github.com/pierrejoubert73/902cc94d79424356a8d20be2b382e1ab
            // details(props) {
            //   const { name, open, children } = props;
            //   debugger;
            //   return <Collapse items={[{ label: name, children: children }]} />;
            // },
            blockquote(props) {
              const { node, children } = props;

              const types = ["success", "info", "warning", "error"] as const;
              type AlertType = (typeof types)[number];
              function isAlertType(str: string): str is AlertType {
                return !!types.find((t) => str === t);
              }

              function getTitle(title: string | undefined, type: AlertType): string {
                if (title) return title;
                if (type === "success") return "Успешно";
                if (type === "info") return "Информация";
                if (type === "warning") return "Предупреждение";
                if (type === "error") return "Ошибка";
                return "";
              }

              var type = node?.properties["type"]?.toString();
              var title = node?.properties["title"]?.toString();
              if (type && isAlertType(type)) {
                return <Alert message={getTitle(title, type)} showIcon description={children} type={type} />;
              }

              return <Text>{children}</Text>;
            },
            kbd(props) {
              const { children } = props;
              return <Text keyboard>{children}</Text>;
            },
            code(props) {
              const { children, className, node } = props;
              const match = /language-(\w+)/.exec(className || "");
              const meta = parseCodeMeta(node?.properties?.meta?.toString());

              let runnable: boolean | undefined;
              if (meta.runnable !== undefined) {
                runnable = meta.runnable === "true";
              }

              return match && className === "language-csharp" ? (
                <CodeExecutor initialCode={children?.toString()} runnable={runnable} />
              ) : (
                <Text code>{children}</Text>
              );
            },
          }}
        >
          {markdown}
        </ReactMarkdown>
      </Content>
    </Layout>
  );
};

export default Md;

function remarkCodeMeta() {
  return (tree: Root) => {
    visit(tree, "code", (node) => {
      if (node.meta) {
        node.data = {
          ...node.data,
          hProperties: {
            // обязательно добавляем в hProperties, иначе удаляется
            ...node.data?.hProperties,
            meta: node.meta, // просто строка, потом будем парсить
          },
        };
      }
    });
  };
}

function parseCodeMeta(meta: string | undefined): any {
  if (!meta) {
    return {};
  }

  return Object.fromEntries(
    meta.split(/\s+/).map((pair) => {
      const [key, value] = pair.split("=");
      return [key, value?.replace(/^['"]|['"]$/g, "")]; // убрать кавычки
    }),
  );
}
