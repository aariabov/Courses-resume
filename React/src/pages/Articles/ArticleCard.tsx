import { css } from "@emotion/react";
import { Card, Flex, theme } from "antd";
import { Link } from "react-router-dom";
import { ArticleRegistryRecord } from "../../api/Api";
import { GetArticleUrl } from "../../helpers/UrlHelper";
import { formatDate } from "../../helpers/commonHelpers";

type ArticleCardProps = {
  article: ArticleRegistryRecord;
};

const ArticleCard: React.FC<ArticleCardProps> = ({ article }) => {
  const { token } = theme.useToken();

  return (
    <Link to={GetArticleUrl(article.url)} style={{ display: "block" }}>
      <Card
        title={
          <Flex>
            <div style={{ color: token.colorLink }}>{article.title}</div>
            <div style={{ marginLeft: "auto", color: token.colorTextTertiary, fontSize: token.fontSizeSM }}>
              {formatDate(article.createDate)}
            </div>
          </Flex>
        }
      >
        <div>{article.shortText}</div>
      </Card>
    </Link>
  );
};

export default ArticleCard;
