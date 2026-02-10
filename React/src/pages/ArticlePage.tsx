import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Article } from "../api/Api";
import { apiClient } from "../api/ApiClient";
import ArticleContent from "../components/ArticleContent";

const ArticlePage: React.FC = () => {
  const { articleUrl } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [article, setArticle] = useState<Article>();

  useEffect(() => {
    if (!articleUrl) {
      navigate(`/not-found`);
      return;
    }

    async function fetchArticle() {
      try {
        const response = await apiClient.api.articleGetArticleByUrl(articleUrl!);
        setArticle(response.data.data);
      } catch (error) {
        console.error("Ошибка при получении цены:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchArticle();
  }, [articleUrl, navigate]);

  const breadcrumbs = article
    ? [
        { title: "Главная", href: "/" },
        { title: "Статьи", href: "/articles" },
        { title: article.title },
      ]
    : undefined;

  return (
    <ArticleContent
      title={`${article?.title} - статьи от Devpull`}
      description={article?.shortText!}
      content={article?.text!}
      loading={loading}
      createDate={article?.createDate}
      breadcrumbs={breadcrumbs}
    />
  );
};

export default ArticlePage;
