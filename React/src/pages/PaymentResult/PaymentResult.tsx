import { Button, Card, Col, Layout, Result, Row, Typography } from "antd";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { apiClient } from "../../api/ApiClient";
import { MARGIN } from "../../constants";
import userStore from "../../stores/UserStore";

const { Text, Title } = Typography;
const { Content } = Layout;

const PaymentResultPage = () => {
  const [isLastPaymentSuccess, setIsLastPaymentSuccess] = useState<boolean>();
  useEffect(() => {
    async function fetchIsLastPaymentSuccess() {
      try {
        const isLastPaymentSuccessResult = await apiClient.api.paymentIsLastPaymentSuccess();
        setIsLastPaymentSuccess(isLastPaymentSuccessResult.data.data);
        userStore.loadUserInfo();
      } catch (error) {
        console.error("Ошибка при получении цены:", error);
      } finally {
        //setLoading(false);
      }
    }

    fetchIsLastPaymentSuccess();
  }, []);

  return (
    <Row justify="center">
      <Col
        style={{ display: "flex" }}
        span={24}
        md={{ span: 21 }}
        lg={{ span: 16 }}
        xl={{ span: 14 }}
        xxl={{ span: 12 }}
      >
        <Content style={{ paddingLeft: MARGIN, paddingRight: MARGIN }}>
          <div style={{ textAlign: "center" }}>
            {isLastPaymentSuccess === true && (
              <Card style={{ border: "none" }}>
                <Result
                  status="success"
                  title="Оплата прошла успешно!"
                  subTitle={<>Спасибо за ваш заказ!</>}
                  extra={[
                    <Button type="primary" key="subscription" size="large">
                      <Link to="/subscription">Моя подписка</Link>
                    </Button>,
                    <Button key="main" size="large">
                      <Link to="/">На главную</Link>
                    </Button>,
                  ]}
                />
              </Card>
            )}
            {isLastPaymentSuccess === false && (
              <Card style={{ border: "none" }}>
                <Result
                  status="warning"
                  title="Оплата не прошла."
                  subTitle={<>Пожалуйста, попробуйте снова или используйте другой способ оплаты.</>}
                  extra={[
                    <Button type="primary" key="subscribe" size="large">
                      <Link to="/subscribe">Попробовать снова</Link>
                    </Button>,
                    <Button key="main" size="large">
                      <Link to="/">На главную</Link>
                    </Button>,
                  ]}
                />
              </Card>
            )}
          </div>
        </Content>
      </Col>
    </Row>
  );
};

export default PaymentResultPage;
