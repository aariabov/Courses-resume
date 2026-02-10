import { theme } from "antd";
import Paragraph from "antd/es/typography/Paragraph";

type CourseCountInfoProps = {
  count: number;
  text: string;
};

const CourseCountInfo: React.FC<CourseCountInfoProps> = ({ count, text }) => {
  const { token } = theme.useToken();

  return (
    <>
      <Paragraph strong style={{ fontSize: token.fontSizeHeading2, marginBottom: 0 }}>
        {count}
      </Paragraph>
      <Paragraph>{text}</Paragraph>
    </>
  );
};

export default CourseCountInfo;
