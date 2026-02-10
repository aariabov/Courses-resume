import { css } from "@emotion/react";
import { Card, Grid, theme } from "antd";
import { Link } from "react-router-dom";
import { CourseRegistryRecord } from "../../api/Api";
import { GetCourseUrl } from "../../helpers/UrlHelper";
import CourseCountInfo from "./CourseCountInfo";

const { useBreakpoint } = Grid;

type CourseCardProps = {
  course: CourseRegistryRecord;
};

const CourseCard: React.FC<CourseCardProps> = ({ course }) => {
  const { token } = theme.useToken();
  const screens = useBreakpoint();

  const cardHeaderClass = css({
    color: token.colorLink,
  });

  const cardClass = css({
    [`:hover .css-${cardHeaderClass.name}`]: {
      color: `${token.colorLinkHover}`,
    },
  });

  // или аналогично из строки
  // const cardClass = css`
  //   &:hover .css-${cardHeaderClass.name} {
  //     color: ${token.colorLinkHover};
  //   }
  // `;

  return (
    <Link
      to={GetCourseUrl(course.url)}
      style={{ display: "block", width: screens.md ? `calc(50% - ${token.padding / 2}px)` : "100%" }}
    >
      <Card
        css={cardClass}
        title={<div css={cardHeaderClass}>{course.name}</div>}
        actions={[
          <CourseCountInfo count={course.stepsCount} text="шагов" />,
          <CourseCountInfo count={course.lessonsCount} text="уроков" />,
        ]}
      >
        <div>{course.shortDescription}</div>
      </Card>
    </Link>
  );
};

export default CourseCard;
