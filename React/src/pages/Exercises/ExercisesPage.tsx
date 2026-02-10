import { GlobalToken, Skeleton, Space, Table, TableColumnsType, theme, Tooltip } from "antd";
import Title from "antd/es/typography/Title";
import React, { useEffect, useState } from "react";
import { Helmet } from "react-helmet";
import { ExerciseRegistryRecord } from "../../api/Api";
import { apiClient } from "../../api/ApiClient";
import ContentLayout from "../../ContentLayout";
import DifficultyProgress from "./DifficultyProgress";
import { GetExerciseUrl } from "../../helpers/UrlHelper";
import { Link } from "react-router-dom";
import { CheckCircleOutlined } from "@ant-design/icons";

const getColumns = (token: GlobalToken): TableColumnsType<ExerciseRegistryRecord> => [
  {
    title: "Is Accepted",
    dataIndex: "isAccepted",
    key: "isAccepted",
    width: "16px",
    onCell: () => ({
      style: { padding: 0 },
    }),
    render: (isAccepted: boolean) => (
      <>
        {isAccepted && (
          <Tooltip title="Решено">
            <CheckCircleOutlined style={{ color: token.colorSuccessText }} />
          </Tooltip>
        )}
      </>
    ),
  },
  {
    title: "Short Name",
    dataIndex: "shortName",
    key: "shortName",
    render: (_, exercise) => (
      <Link to={GetExerciseUrl(exercise.url)}>{`${exercise.number}. ${exercise.shortName}`}</Link>
    ),
    sorter: (a: ExerciseRegistryRecord, b: ExerciseRegistryRecord) =>
      (a.shortName ?? "").localeCompare(b.shortName ?? ""),
  },
  {
    title: "Level",
    dataIndex: "level",
    key: "level",
    align: "right",
    render: (level: string) => <DifficultyProgress level={level} />,
  },
];

const ExercisesPage: React.FC = () => {
  const [exercises, setExercises] = useState<ExerciseRegistryRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const { token } = theme.useToken();

  useEffect(() => {
    async function fetchExercises() {
      try {
        const response = await apiClient.api.exerciseGetExercises();
        setExercises(response.data.data);
      } catch (error) {
        console.error("Ошибка при получении упражнений:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchExercises();
  }, []);

  return (
    <>
      <Helmet>
        <title>Упражнения от Devpull</title>
        <meta name="description" content={`Каталог практических упражнений по программированию.`} />
      </Helmet>
      <ContentLayout>
        <Skeleton loading={loading} title active paragraph={{ rows: 21 }}>
          <Title>Упражнения</Title>
          {/*<Space direction="vertical" size="middle">*/}
          {/*  {exercises.map((ex) => (*/}
          {/*    <ExerciseCard exercise={ex} key={ex.id} />*/}
          {/*  ))}*/}
          {/*</Space>*/}
          <Table<ExerciseRegistryRecord>
            columns={getColumns(token)}
            showHeader={false}
            dataSource={exercises}
            rowKey="id"
            pagination={false}
          />
        </Skeleton>
      </ContentLayout>
    </>
  );
};

export default ExercisesPage;
