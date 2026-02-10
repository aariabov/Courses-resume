import React from "react";
import { Card, Typography, Divider, List, Descriptions, DescriptionsProps } from "antd";
import { ExerciseExampleDto, ExerciseRegistryRecord } from "../../../api/Api";

const { Title, Text } = Typography;

type ExampleCardProps = {
  example: ExerciseExampleDto;
  idx: number;
};

const items: DescriptionsProps['items'] = [
  {
    key: '1',
    label: 'UserName',
    children: <p>Zhou Maomao</p>,
  },
  {
    key: '2',
    label: 'Telephone',
    children: <p>1810000000</p>,
  },
  {
    key: '3',
    label: 'Live',
    children: <p>Hangzhou, Zhejiang</p>,
  }
];

const ExampleCard: React.FC<ExampleCardProps> = ({ example, idx }) => {
  return (
    <Typography.Paragraph>
      <Text strong>{`Пример ${idx + 1}`}</Text>
      <div
        style={{
          borderLeft: "4px solid #d9d9d9",
          paddingLeft: "12px",
          marginTop: "8px",
          color: "#555",
        }}
      >
        <Text >Вход: </Text><Text>{example.input}</Text><br />
        <Text >Выход: </Text><Text>{example.output}</Text><br />
        <Text >Объяснение: </Text><Text>{example.explanation}</Text>
      </div>
    </Typography.Paragraph>
  );
};

export default ExampleCard;