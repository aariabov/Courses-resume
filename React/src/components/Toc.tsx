import { Anchor, theme } from "antd";
import { AnchorLinkItemProps } from "antd/es/anchor/Anchor";
import React, { useEffect, useState } from "react";
import rehypeStringify from "rehype-stringify";
import { remark } from "remark";
import remarkFlexibleToc, { HeadingDepth, TocItem } from "remark-flexible-toc";
import gfm from "remark-gfm";
import remarkRehype from "remark-rehype";

type MarkdownProps = {
  markdown: string;
};

interface MyAnchorLinkItemProps extends AnchorLinkItemProps {
  depth: HeadingDepth;
}

const Toc: React.FC<MarkdownProps> = ({ markdown }) => {
  const { token } = theme.useToken();
  const [contentItems, setContentItems] = useState<AnchorLinkItemProps[]>([]);

  useEffect(() => {
    const tocItems: TocItem[] = [];

    remark()
      .use(gfm)
      .use(remarkFlexibleToc, { tocRef: tocItems })
      .use(remarkRehype)
      .use(rehypeStringify)
      .processSync(markdown);

    var stack: MyAnchorLinkItemProps[] = [];
    var items: MyAnchorLinkItemProps[] = [];

    tocItems.forEach((item) => {
      var anchor: MyAnchorLinkItemProps = {
        key: item.value,
        href: item.href,
        title: item.value,
        depth: item.depth,
        children: [],
      };

      var minLevel = Math.min(...tocItems.map((x) => x.depth));
      if (anchor.depth === minLevel) {
        items.push(anchor);
      } else {
        addItem(anchor);
      }
      stack.push(anchor);
    });

    function addItem(item: MyAnchorLinkItemProps) {
      var lastItem = stack[stack.length - 1];
      if (item.depth > lastItem.depth) {
        // идем вглубь - просто добавляем детей
        lastItem.children?.push(item);
      } else if (item.depth < lastItem.depth) {
        // поднимаемся наверх, до родителя
        while (lastItem.depth !== item.depth) {
          lastItem = stack.pop()!;
        }
        lastItem = stack[stack.length - 1];
        lastItem.children?.push(item);
      } else {
        // елемент того же уровня
        stack.pop()!; // удаляем елемент того же уровня
        lastItem = stack[stack.length - 1]; // берем парента
        lastItem.children?.push(item);
      }
    }
    setContentItems(items);
  }, [markdown]);

  return <Anchor items={contentItems} style={{ paddingRight: token.paddingXL }} />;
};

export default Toc;
