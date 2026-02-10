import React from "react";
import ArticleContent from "../../components/ArticleContent";
import Offer from "./Offer.md";

const OfferPage: React.FC = () => {
  return (
    <ArticleContent
      title="Договор-оферта о предоставлении доступа к онлайн-курсам Devpull"
      description="Официальный договор-оферта на оказание услуг по предоставлению доступа к онлайн-курсам Devpull. Условия подписки, оплаты и возврата."
      content={Offer}
      loading={false}
    />
  );
};

export default OfferPage;
