import { Card, Collapse, theme, Typography } from "antd";
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { RunExerciseHistoryDto, TestStatusView } from "../../../api/Api";
import { apiClient } from "../../../api/ApiClient";
import { formatDateTime, getStatusText } from "../../../helpers/commonHelpers";
import HistoryItem from "./HistoryItem";

const { Panel } = Collapse;

// small helper to render header: date + status
const renderHeader = (date: string, status: TestStatusView) => (
  <div style={{ display: "flex", justifyContent: "space-between", width: "100%" }}>
    <span>{formatDateTime(date)}</span>
    <Typography.Text type={status === "Success" ? "success" : "danger"}>{getStatusText(status)}</Typography.Text>
  </div>
);

type HistoryTabProps = {
  exerciseId: string;
};

const HistoryTab: React.FC<HistoryTabProps> = ({ exerciseId }) => {
  theme.useToken();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [result, setResult] = useState<string>("");
  const [runExercises, setRunExercises] = useState<RunExerciseHistoryDto[]>();

  useEffect(() => {
    if (!exerciseId) {
      navigate(`/not-found`);
      return;
    }

    async function fetchArticle() {
      try {
        const response = await apiClient.api.exerciseGetRunExerciseHistory(exerciseId);
        setRunExercises(response.data.data);
      } catch (error) {
        console.error("Ошибка при получении упражнения:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchArticle();
  }, [exerciseId, navigate]);

  return (
    <Collapse accordion>
      {(runExercises ?? []).map((r) => (
        <Collapse.Panel header={renderHeader(r.date, r.result.status)} key={r.id}>
          <HistoryItem code={r.code} resultView={r.result} functionName={r.functionName} />
        </Collapse.Panel>
      ))}
    </Collapse>
  );
};

export default HistoryTab;
