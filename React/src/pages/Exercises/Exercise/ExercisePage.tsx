import { Grid, Layout, Splitter } from "antd";
import React, { useEffect, useState } from "react";
import { Helmet } from "react-helmet";
import { ExerciseDto } from "../../../api/Api";
import { apiClient } from "../../../api/ApiClient";
import { useNavigate, useParams } from "react-router-dom";
import ExerciseCard from "./ExerciseCard";
import ExerciseTester from "./ExerciseTester";
import Breadcrumbs from "../../../components/Breadcrumbs";

const { useBreakpoint } = Grid;

const ExercisePage: React.FC = () => {
  const { exerciseUrl } = useParams();
  const screens = useBreakpoint();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [exercise, setExercise] = useState<ExerciseDto>();

  useEffect(() => {
    if (!exerciseUrl) {
      navigate(`/not-found`);
      return;
    }

    async function fetchArticle() {
      try {
        const response = await apiClient.api.exerciseGetExerciseByUrl(exerciseUrl!);
        const exersice = response.data.data;
        setExercise(exersice);
      } catch (error) {
        console.error("Ошибка при получении упражнения:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchArticle();
  }, [exerciseUrl, navigate]);

  const breadcrumbs = exercise
    ? [
        { title: "Главная", href: "/" },
        { title: "Упражнения", href: "/exercises" },
        { title: `${exercise.number}. ${exercise.shortName}` },
      ]
    : undefined;

  return (
    <>
      <Helmet>
        <title>{`Упражнение ${exercise?.shortName} от Devpull`}</title>
        <meta name="description" content={exercise?.description} />
      </Helmet>
      <Layout style={{ height: "100%" }}>
        <div style={{ margin: "0 16px" }}>
          <Breadcrumbs customItems={breadcrumbs} />
        </div>
        <Splitter layout={screens.md ? "horizontal" : "vertical"} style={{ boxShadow: "0 0 10px rgba(0, 0, 0, 0.1)" }}>
          <Splitter.Panel min="30%">
            {exercise && <ExerciseCard exercise={exercise} loading={loading} />}
          </Splitter.Panel>
          <Splitter.Panel min="30%" style={{ overflow: "hidden" }}>
            {exercise && (
              <ExerciseTester
                exercise={exercise}
                onAccepted={() => {
                  setExercise((prev) => (prev ? { ...prev, isAccepted: true } : prev));
                }}
              />
            )}
          </Splitter.Panel>
        </Splitter>
      </Layout>
    </>
  );
};

export default ExercisePage;
