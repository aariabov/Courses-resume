import { Card, List, theme, Typography } from "antd";
import React from "react";
import { Link } from "react-router-dom";
import { StepDto } from "../../api/Api";
import { GetLessonUrl, GetStepUrl } from "../../helpers/UrlHelper";

type StepCardProps = {
  step: StepDto;
  stepIndex: number;
  courseUrl: string;
};

const { Text } = Typography;

const StepCard: React.FC<StepCardProps> = ({ step, stepIndex, courseUrl }) => {
  const { token } = theme.useToken();

  return (
    <Card
      key={step.id}
      style={{ marginBottom: token.margin, padding: 0 }}
      styles={{ body: { paddingTop: 0, paddingBottom: 0 } }}
      title={
        <Link to={GetStepUrl(courseUrl, step.url)}>
          <Text strong>{`Шаг ${stepIndex + 1}. ${step.name}`}</Text>
        </Link>
      }
    >
      <List
        key={step.id}
        size="large"
        dataSource={step.lessons}
        renderItem={(lesson) => (
          <List.Item>
            <Link to={GetLessonUrl(courseUrl, step.url, lesson.url)}>{lesson.name}</Link>
          </List.Item>
        )}
      />
    </Card>
  );
};

export default StepCard;
