import { Alert, Button, Col, Divider, Flex, Grid, Layout, Menu, Row, Skeleton, Space, Typography } from "antd";
import Sider from "antd/es/layout/Sider";
import { SubMenuType } from "antd/es/menu/interface";
import Title from "antd/es/typography/Title";
import { observer } from "mobx-react-lite";
import React, { useEffect, useRef, useState } from "react";
import { Helmet } from "react-helmet";
import { Link, useNavigate, useParams } from "react-router-dom";
import { CourseDto, LessonDto, StepDto } from "../api/Api";
import { apiClient } from "../api/ApiClient";
import FunctionTester from "../components/FunctionTester";
import Md from "../components/Md";
import { MARGIN } from "../constants";
import userStore from "../stores/UserStore";
import Paragraph from "antd/es/typography/Paragraph";
import Breadcrumbs from "../components/Breadcrumbs";
import ExerciseCard from "./Exercises/Exercise/ExerciseCard";
import ExampleCard from "./Exercises/Exercise/ExampleCard";

const { Content } = Layout;
const { useBreakpoint } = Grid;

const LessonPage: React.FC = observer(() => {
  const { courseUrl, stepUrl, lessonUrl } = useParams();
  const navigate = useNavigate();
  const scrollableRef = useRef<HTMLDivElement>(null);
  const [selectedKeys, setSelectedKeys] = useState<string[]>([]);
  const [openKeys, setOpenKeys] = useState<string[]>([]);
  const [course, setCourse] = useState<CourseDto>();
  const [lesson, setLesson] = useState<LessonDto>();
  const [items, setItems] = useState<SubMenuType[]>();
  const [prevUrl, setPrevUrl] = useState<string>();
  const [nextUrl, setNextUrl] = useState<string>();
  const screens = useBreakpoint();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLesson(undefined);
    setLoading(true);
  }, [lessonUrl]);

  useEffect(() => {
    setLesson(undefined);

    async function fetchCourse() {
      try {
        const courseResponse = await apiClient.api.courseGetCourseByUrl({
          courseUrl: courseUrl!,
          lessonUrl: lessonUrl ?? null,
        });
        setCourse(courseResponse.data.data);
      } catch (error) {
        console.error("Ошибка при получении курса:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchCourse();
  }, [courseUrl, lessonUrl]);

  useEffect(() => {
    if (course) {
      let step: StepDto | undefined = course.steps.find((s) => s.url === stepUrl);
      if (stepUrl && !step) {
        navigate(`/not-found`);
        return;
      }

      const items: SubMenuType[] = [];
      const allLessons: { step: StepDto; lesson: LessonDto }[] = [];

      course.steps.forEach((currentStep, idx) => {
        let item: SubMenuType = {
          key: currentStep.url,
          label: `Шаг ${idx + 1}: ${currentStep.name}`,
          children: [],
        };

        currentStep.lessons.forEach((currentLesson) => {
          allLessons.push({
            step: currentStep,
            lesson: currentLesson,
          });

          item.children.push({
            key: `${currentStep.url}__${currentLesson.url}`,
            label: (
              <Link to={`/courses/${course.url}/${currentStep.url}/${currentLesson.url}`}>{currentLesson.name}</Link>
            ),
          });
        });
        items.push(item);
      });

      const foundLesson = allLessons.find((u) => u.step.url === stepUrl && u.lesson.url === lessonUrl);
      if (!foundLesson) {
        navigate(`/not-found`);
        return;
      }

      const foundLessonIdx = allLessons.indexOf(foundLesson);
      if (foundLessonIdx > 0) {
        const prevLesson = allLessons[foundLessonIdx - 1];
        setPrevUrl(`/courses/${course.url}/${prevLesson.step.url}/${prevLesson.lesson.url}`);
      } else {
        setPrevUrl(undefined);
      }

      if (foundLessonIdx < allLessons.length - 1) {
        const nextLesson = allLessons[foundLessonIdx + 1];
        setNextUrl(`/courses/${course.url}/${nextLesson.step.url}/${nextLesson.lesson.url}`);
      } else {
        setNextUrl(undefined);
      }

      setLesson(foundLesson.lesson);
      if (!openKeys.includes(foundLesson.step.url)) {
        openKeys.push(foundLesson.step.url);
      }
      setSelectedKeys([`${foundLesson.step.url}__${foundLesson.lesson.url}`]);
      setItems(items);
      scrollableRef.current?.scrollTo(0, 0);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [navigate, course, stepUrl, lessonUrl]); // с openKeys не работает - до конца не понял почему

  let siderWidth = "0%";
  if (screens.md) {
    siderWidth = "30%";
  }
  if (screens.xl) {
    siderWidth = "20%";
  }

  const breadcrumbs =
    course && lesson
      ? [
          { title: "Главная", href: "/" },
          { title: "Курсы", href: "/courses" },
          { title: course.name, href: `/courses/${course.url}` },
          {
            title: `Шаг ${course.steps.findIndex((s) => s.url === stepUrl) + 1}. ${course.steps.find((s) => s.url === stepUrl)?.name}`,
            href: `/courses/${course.url}/${stepUrl}`,
          },
          { title: lesson.name },
        ]
      : undefined;

  return (
    <>
      <Helmet>
        <title>{`Урок "${lesson?.name}" - курс ${course?.name} от Devpull`}</title>
        <meta
          name="description"
          content={`Урок "${lesson?.name}" курса ${course?.name}. Краткая теория. Практика. Выполнение упражнений.`}
        />
      </Helmet>
      <Layout style={{ height: "100%" }}>
        <Sider width={siderWidth} theme="light" style={{ height: "100%", overflowY: "auto" }}>
          {loading ? (
            <Skeleton active paragraph={{ rows: 10 }} />
          ) : (
            items &&
            selectedKeys.length > 0 && (
              <Menu
                selectedKeys={selectedKeys}
                openKeys={openKeys}
                mode="inline"
                items={items}
                onOpenChange={(openKeys: string[]) => setOpenKeys(openKeys)}
              />
            )
          )}
        </Sider>
        <Row ref={scrollableRef} justify="center" style={{ width: "100%", height: "100%", overflowY: "scroll" }}>
          <Col span={24} xl={{ span: 16 }}>
            <Content style={{ margin: `0 ${MARGIN} ${MARGIN}` }}>
              {loading ? (
                <Skeleton active paragraph={{ rows: 15 }} />
              ) : (
                lesson?.content && (
                  <>
                    <Breadcrumbs customItems={breadcrumbs} />
                    <Md markdown={lesson.content} showToc={false} />
                    {lesson.exercise && (
                      <>
                        <Md markdown={lesson.exercise.description} showToc={false} />

                        {lesson.exercise.examples.map((ex, idx) => {
                          const key = `${ex.input ?? ""}::${ex.output ?? ""}`;
                          return <ExampleCard key={key} idx={idx} example={ex} />;
                        })}
                        {!userStore.isLogged && (
                          <Alert
                            message={
                              <>
                                <Typography.Text>Для выполнения упражнений необходимо </Typography.Text>
                                <Button type="link" size="small" onClick={() => userStore.showLoginForm()}>
                                  войти
                                </Button>
                              </>
                            }
                            type="warning"
                          />
                        )}
                        <Paragraph style={{ marginTop: MARGIN }}>
                          <FunctionTester key={lesson.exercise.id} idx={0} exercise={lesson.exercise} />
                        </Paragraph>
                      </>
                    )}
                    <Divider />
                    <Flex justify="space-between">
                      <Button type="primary" ghost style={{ visibility: prevUrl ? "visible" : "hidden" }}>
                        {prevUrl && <Link to={prevUrl}>{"< Назад"}</Link>}
                      </Button>
                      <Button type="primary" ghost style={{ visibility: nextUrl ? "visible" : "hidden" }}>
                        {nextUrl && <Link to={nextUrl}>{"Далее >"}</Link>}
                      </Button>
                    </Flex>
                  </>
                )
              )}
            </Content>
          </Col>
        </Row>
      </Layout>
    </>
  );
});

export default LessonPage;
