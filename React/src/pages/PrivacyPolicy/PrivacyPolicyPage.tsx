import React from "react";
import ArticleContent from "../../components/ArticleContent";
import PrivacyPolicy from "./PrivacyPolicy.md";

const PrivacyPolicyPage: React.FC = () => {
  return (
    <ArticleContent
      title="Политика конфиденциальности онлайн-курсов Devpull"
      description="Политика конфиденциальности: правила сбора, хранения и защиты данных пользователей сайта devpull.courses"
      content={PrivacyPolicy}
      loading={false}
    />
  );
};

export default PrivacyPolicyPage;
