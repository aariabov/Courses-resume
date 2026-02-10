import {
  CodeOutlined,
  DislikeOutlined,
  InfoCircleOutlined,
  LaptopOutlined,
  LikeOutlined,
  QuestionCircleOutlined,
  TeamOutlined,
} from "@ant-design/icons";
import { Col, Row, Steps, theme, Typography } from "antd";
import Paragraph from "antd/es/typography/Paragraph";
import React from "react";
import { Helmet } from "react-helmet";
import Md from "../../components/Md";
import ContentLayout from "../../ContentLayout";
import MainPageContent from "../../pages/content/MainPage.md";

const { Text } = Typography;

const MainPage: React.FC = () => {
  const { token } = theme.useToken();

  return (
    <>
      <Helmet>
        <title>ИТ-курсы: онлайн обучение программированию от Devpull</title>
        <meta
          name="description"
          content="Онлайн курсы программирования для начинающих и специалистов по системе Суперобучение."
        />
      </Helmet>
      <ContentLayout>
        <Md markdown={MainPageContent} showToc={false} />
        <Row gutter={16}>
          <Col
            style={{
              marginTop: token.margin,
              marginBottom: token.marginMD,
            }}
            span={24}
            sm={{ span: 12 }}
          >
            <Paragraph strong>Суперобучение</Paragraph>
            <Steps
              direction="vertical"
              size="small"
              current={5}
              items={[
                {
                  title: "Практика",
                  description: "Рассмотрение практического примера",
                  icon: <LaptopOutlined />,
                },
                {
                  title: "Теория",
                  description: "Детальный разбор примера, теория",
                  icon: <InfoCircleOutlined />,
                },
                {
                  title: "Много упражнений",
                  description: "Самостоятельная практическая работа",
                  icon: <CodeOutlined />,
                },
                {
                  title: "Обратная связь",
                  description: "Что и как можно улучшить",
                  icon: <TeamOutlined />,
                },
                {
                  title: "Результат",
                  description: "Достижение высокого результата",
                  icon: <LikeOutlined />,
                },
              ]}
            />
          </Col>
          <Col
            style={{
              marginTop: token.margin,
              marginBottom: token.marginMD,
            }}
            span={24}
            sm={{ span: 12 }}
          >
            <Paragraph strong>Традиционный процесс обучения</Paragraph>
            <Steps
              direction="vertical"
              size="small"
              current={4}
              items={[
                {
                  title: "Теория",
                  description: "Большой объем скучной теории",
                  icon: <InfoCircleOutlined />,
                },
                {
                  title: "Упражнения",
                  description: "Самостоятельная практическая работа",
                  icon: <CodeOutlined />,
                },
                {
                  title: "Проверка знаний",
                  description: "Экзамен, тест или контрольная работа",
                  icon: <QuestionCircleOutlined />,
                },
                {
                  title: "Результат",
                  description: "Посредственный результат",
                  icon: <DislikeOutlined />,
                },
              ]}
            />
          </Col>
        </Row>
        <Row>
          <Paragraph>
            Как правило, в традиционной системе обучения, не учитывается индивидуальный уровень знаний, студенты имеют
            слабую мотивацию, а сам процесс сводится к запоминанию фактов. Отсюда посредственные результаты.
          </Paragraph>
          <Paragraph>
            Система <Text strong>Суперобучения</Text> лишена этих недостатков, процесс обучения получается интересным и
            эффективным. Студенты получают позитивный опыт и показывают более высокие результаты.
          </Paragraph>
          <Paragraph>
            Наши курсы построены исключительно по системе <Text strong>Суперобучения</Text>. Статьи написаны кратко и
            структурировано. Надеемся наш подход будет полезен для вас, а процесс обучения станет увлекательным и
            захватывающим.
          </Paragraph>
        </Row>
      </ContentLayout>
    </>
  );
};

export default MainPage;
