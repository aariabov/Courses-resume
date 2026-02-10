import React from "react";
import { Card, Flex, theme, Tooltip, Typography, Tabs, Table } from "antd";
import { ExerciseDto } from "../../../api/Api";
import Md from "../../../components/Md";
import ExampleCard from "./ExampleCard";
import DifficultyProgress from "../DifficultyProgress";
import { CheckCircleOutlined } from "@ant-design/icons";
import Title from "antd/es/typography/Title";
import HistoryTab from "./HistoryTab";

type ExerciseCardProps = {
  exercise: ExerciseDto;
  loading: boolean;
};

const ExerciseCard: React.FC<ExerciseCardProps> = ({ exercise, loading }) => {
  const { token } = theme.useToken();

  return (
    <Card
      variant="borderless"
      loading={loading}
      styles={{
        header: {
          borderBottom: "none",
        },
        body: {
          paddingTop: 0,
        },
      }}
      style={{
        boxShadow: "none",
      }}
    >
      <Tabs defaultActiveKey="description">
        <Tabs.TabPane tab="Описание" key="description">
          <Flex gap={10} align="center">
            <Title level={4} style={{ marginTop: 0 }}>{`${exercise.number}. ${exercise.shortName}`}</Title>
            {exercise.isAccepted && (
              <Tooltip title="Решено">
                <CheckCircleOutlined style={{ color: token.colorSuccessText }} />
              </Tooltip>
            )}
            <div style={{ marginLeft: "auto" }}>{<DifficultyProgress level={exercise.level} />}</div>
          </Flex>
          <Typography>
            <Md markdown={exercise.description} showToc={false} />

            {exercise.examples.map((ex, idx) => {
              const key = `${ex.input ?? ""}::${ex.output ?? ""}`;
              return <ExampleCard key={key} idx={idx} example={ex} />;
            })}
          </Typography>
        </Tabs.TabPane>

        <Tabs.TabPane tab="История" key="history">
          <HistoryTab exerciseId={exercise.id} />
        </Tabs.TabPane>
      </Tabs>
    </Card>
  );
};

export default ExerciseCard;
