import { css } from "@emotion/react";
import { List, theme, Typography } from "antd";
import Paragraph from "antd/es/typography/Paragraph";
import React from "react";
import { Link, useParams } from "react-router-dom";
import { CourseDto, StepDto } from "../../api/Api";
import { GetCourseUrl, GetStepUrl } from "../../helpers/UrlHelper";

type CourseMainPageProps = {
  course: CourseDto;
};

const { Text } = Typography;

const CourseMainPageMenu: React.FC<CourseMainPageProps> = ({ course }) => {
  const { stepUrl } = useParams();
  const { token } = theme.useToken();
  let step: StepDto | undefined = course.steps.find((s) => s.url === stepUrl);

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
      header={
        <Link to={GetCourseUrl(course.url)} style={{ display: "block" }}>
          <Text>Обзор курса</Text>
        </Link>
      }
      bordered
      dataSource={course.steps}
      renderItem={(s, idx) => (
        <List.Item
          css={itemClass}
          style={{
            backgroundColor: s.url === step?.url ? token.controlItemBgActive : undefined,
          }}
        >
          <Link
            to={GetStepUrl(course.url, s.url)}
            style={{
              width: "100%",
              padding: `${token.paddingContentVerticalLG}px ${token.paddingContentHorizontalLG}px`,
            }}
          >
            <Text>{`Шаг ${idx + 1}. ${s.name}`}</Text>
            <Paragraph type="secondary" ellipsis={{ rows: 2 }} style={{ fontSize: token.fontSizeSM }}>
              {s.shortDescription}
            </Paragraph>
          </Link>
        </List.Item>
      )}
    />
  );
};

export default CourseMainPageMenu;
