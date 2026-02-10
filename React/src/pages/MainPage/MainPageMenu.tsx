import { css } from "@emotion/react";
import { List, theme, Typography } from "antd";
import Paragraph from "antd/es/typography/Paragraph";
import React from "react";
import { Link } from "react-router-dom";
import { ArraysCourseUrl, CSharpKidsCourseUrl } from "../../constants";
import { GetArticleUrl, GetCourseUrl } from "../../helpers/UrlHelper";

const { Text } = Typography;

interface Item {
  title: string;
  shortDescription: string;
  url: string;
}

const data: Item[] = [
  {
    title: "Курс C# Kids",
    shortDescription: "Курс по C# для начинающих",
    url: GetCourseUrl(CSharpKidsCourseUrl),
  },
  {
    title: "Курс Массивы C#",
    shortDescription: "Курс по массивам C#. Теория, практика, задачи с автоматической проверкой.",
    url: GetCourseUrl(ArraysCourseUrl),
  },
  {
    title: "Слепая печать",
    shortDescription: "Слепая десятипальцевая печать — метод набора без взгляда на клавиатуру",
    url: GetArticleUrl("blind-typing"),
  },
  {
    title: "О сервисе",
    shortDescription: "Про философию и ценности сервиса",
    url: GetArticleUrl("about"),
  },
  {
    title: "Суперобучение",
    shortDescription: "Революционная система обучения, нацеленная на скорость и результат",
    url: GetArticleUrl("super-education"),
  },
  {
    title: "Советы разработчику",
    shortDescription: "Какими хард-скиллами и софт-скиллами должен обладать профессиональный разработчик",
    url: GetArticleUrl("developer-tips"),
  },
];

const MainPageMenu: React.FC = () => {
  const { token } = theme.useToken();

  const itemClass = css({
    padding: "0 !important",
    ":hover": {
      backgroundColor: token.controlItemBgActive,
      transition: "background-color 0.3s linear;",
    },
  });

  return (
    <List
      size="large"
      style={{ width: "100%" }}
      header="Контент"
      bordered
      dataSource={data}
      renderItem={(item, idx) => (
        <List.Item css={itemClass}>
          <Link
            to={item.url}
            style={{
              width: "100%",
              padding: `${token.paddingContentVerticalLG}px ${token.paddingContentHorizontalLG}px`,
            }}
          >
            <Text>{item.title}</Text>
            <Paragraph type="secondary" ellipsis={{ rows: 2 }} style={{ fontSize: token.fontSizeSM }}>
              {item.shortDescription}
            </Paragraph>
          </Link>
        </List.Item>
      )}
    />
  );
};

export default MainPageMenu;
