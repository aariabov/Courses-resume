import React from "react";
import { Button, Result } from "antd";
import { Helmet } from "react-helmet";

const NotFound: React.FC = () => (
  <>
    <Helmet>
      <title>Страница не найдена. Ошибка 404.</title>
      <meta name="description" content="Страница не найдена. Вернитесь на главную или воспользуйтесь меню." />
    </Helmet>
    <Result
      status="404"
      title="404"
      subTitle="Страница не найдена."
      extra={
        <Button type="primary" href="/">
          На главную
        </Button>
      }
    />
  </>
);

export default NotFound;
