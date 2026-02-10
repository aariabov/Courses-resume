import { css } from "@emotion/react";
import { Card, Flex, theme } from "antd";
import { Link } from "react-router-dom";
import { ArticleRegistryRecord, ExerciseRegistryRecord } from "../../api/Api";
import { GetArticleUrl } from "../../helpers/UrlHelper";
import { formatDate } from "../../helpers/commonHelpers";
import DifficultyScale from "./DifficultyScale";
import DifficultyProgress from "./DifficultyProgress";
import DifficultyMeter from "./DifficultyMeter";
import Md from "../../components/Md";

type ExerciseCardProps = {
  exercise: ExerciseRegistryRecord;
};

const ExerciseCard: React.FC<ExerciseCardProps> = ({ exercise }) => {
  const { token } = theme.useToken();

  return (
    <Link to={GetArticleUrl(exercise.id)} style={{ display: "block" }}>
      <Card
        title={
          <Flex>
            <div style={{ color: token.colorLink }}>{exercise.shortName}</div>
            <div style={{ marginLeft: "auto", color: token.colorTextTertiary, fontSize: token.fontSizeSM }}>
              {<DifficultyProgress level={exercise.level} />}
            </div>
          </Flex>
        }
      >
        <Md markdown={exercise.shortName!} showToc={false} />
      </Card>
    </Link>
  );
};

export default ExerciseCard;
