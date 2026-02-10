import { Col, Row, Skeleton } from "antd";
import React, { useEffect, useState } from "react";
import { Helmet } from "react-helmet";
import { useNavigate, useParams } from "react-router-dom";
import { CourseDto, StepDto } from "../../api/Api";
import { apiClient } from "../../api/ApiClient";
import Md from "../../components/Md";
import ContentLayout from "../../ContentLayout";
import CourseMainPageMenu from "./CourseMainPageMenu";
import StepCard from "./StepCard";

const CourseMainPage: React.FC = () => {
  const { courseUrl, stepUrl } = useParams();
  const navigate = useNavigate();
  const [course, setCourse] = useState<CourseDto>();
  const [courseDescription, setCourseDescription] = useState<string>("");
  const [title, setTitle] = useState<string>("");
  const [description, setDescription] = useState<string>("");
  const [steps, setSteps] = useState<StepDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchCourse() {
      try {
        const response = await apiClient.api.courseGetCourseByUrl({ courseUrl: courseUrl!, lessonUrl: null });
        setCourse(response.data.data);
      } catch (error) {
        console.error("Ошибка при получении курса:", error);
        navigate(`/not-found`);
        return;
      } finally {
        setLoading(false);
      }
    }

    fetchCourse();
  }, [courseUrl, navigate]);

  useEffect(() => {
    if (course) {
      let step: StepDto | undefined = course.steps.find((s) => s.url === stepUrl);
      if (stepUrl && !step) {
        navigate(`/not-found`);
        return;
      }

      if (step) {
        setCourseDescription(step.description);
        setSteps([step]);

        let stepIndex = course.steps.indexOf(step);
        setTitle(`Шаг ${stepIndex + 1}. ${step.name} - курс ${course?.name} от Devpull`);
        setDescription(step.shortDescription);
      }
      else {
        setCourseDescription(course.description);
        setSteps(course.steps);
        setTitle(`Курс "${course.name}" от Devpull`);
        setDescription(course.description);
      }
    }
  }, [course, navigate, stepUrl]);

  const breadcrumbs = course
    ? [
        { title: "Главная", href: "/" },
        { title: "Курсы", href: "/courses" },
        { title: course.name, href: `/courses/${course.url}` },
        ...(stepUrl
          ? [
              {
                title:
                  `Шаг ${course.steps.findIndex((s) => s.url === stepUrl) + 1}. ${course.steps.find((s) => s.url === stepUrl)?.name}`,
              },
            ]
          : []),
      ]
    : undefined;

  return (
    <>
      <Helmet>
        <title>{title}</title>
        <meta name="description" content={description} />
      </Helmet>

      <ContentLayout
        menu={course ? <CourseMainPageMenu course={course} /> : <Skeleton active paragraph={{ rows: 10 }} />}
        breadcrumbs={breadcrumbs}
      >
        {loading ? (
          <Skeleton active paragraph={{ rows: 21 }} />
        ) : (
          course && (
            <>
              <Md markdown={courseDescription} showToc={false} />
              <Row gutter={16}>
                <Col span={24} md={{ span: 12 }}>
                  {steps
                    .filter((_, i) => i < Math.ceil(steps.length / 2))
                    .map((s) => (
                      <StepCard key={s.id} step={s} stepIndex={course.steps.indexOf(s)} courseUrl={course.url} />
                    ))}
                </Col>
                <Col span={24} md={{ span: 12 }}>
                  {steps
                    .filter((_, i) => i >= steps.length / 2)
                    .map((s) => (
                      <StepCard key={s.id} step={s} stepIndex={course.steps.indexOf(s)} courseUrl={course.url} />
                    ))}
                </Col>
              </Row>
            </>
          )
        )}
      </ContentLayout>
    </>
  );
};

export default CourseMainPage;
