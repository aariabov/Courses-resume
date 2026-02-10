import { Flex, Skeleton } from "antd";
import Title from "antd/es/typography/Title";
import { useEffect, useState } from "react";
import { Helmet } from "react-helmet";
import { CourseRegistryRecord } from "../../api/Api";
import { apiClient } from "../../api/ApiClient";
import ContentLayout from "../../ContentLayout";
import CourseCard from "./CourseCard";

const CoursesPage: React.FC = () => {
  const [courses, setCourses] = useState<CourseRegistryRecord[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchCourses() {
      try {
        const response = await apiClient.api.courseGetCourses();
        setCourses(response.data.data);
      } catch (error) {
        console.error("Ошибка при получении курсов:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchCourses();
  }, []);

  return (
    <>
      <Helmet>
        <title>Онлайн курсы программирования от Devpull</title>
        <meta
          name="description"
          content="Выбирайте свой онлайн курс по программированию. Учитывайте свой уровень, опыт и знания. Учитесь в удобное время и в своем темпе."
        />
      </Helmet>
      <ContentLayout>
        <Skeleton loading={loading} title active paragraph={{ rows: 21 }}>
          <Title>Курсы</Title>
          <Flex gap="middle" wrap>
            {courses.map((course) => (
              <CourseCard key={course.id} course={course} />
            ))}
          </Flex>
        </Skeleton>
      </ContentLayout>
    </>
  );
};

export default CoursesPage;
