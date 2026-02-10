import { Skeleton, Space } from "antd";
import Title from "antd/es/typography/Title";
import { useEffect, useState } from "react";
import { Helmet } from "react-helmet";
import { ArticleRegistryRecord } from "../../api/Api";
import { apiClient } from "../../api/ApiClient";
import ContentLayout from "../../ContentLayout";
import ArticleCard from "./ArticleCard";

const ArticlesPage: React.FC = () => {
  const [articles, setArticles] = useState<ArticleRegistryRecord[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchArticles() {
      try {
        const response = await apiClient.api.articleGetArticles();
        setArticles(response.data.data);
      } catch (error) {
        console.error("Ошибка при получении статей:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchArticles();
  }, []);

  return (
    <>
      <Helmet>
        <title>Статьи от Devpull</title>
        <meta
          name="description"
          content={`Полезные статьи для программистов от основ до продвинутых тем. Гайды, советы и лучшие практики.`}
        />
      </Helmet>
      <ContentLayout>
        <Skeleton loading={loading} title active paragraph={{ rows: 21 }}>
          <Title>Статьи</Title>
          <Space direction="vertical" size="large">
            {articles.map((article) => (
              <ArticleCard key={article.id} article={article} />
            ))}
          </Space>
        </Skeleton>
      </ContentLayout>
    </>
  );
};

export default ArticlesPage;
