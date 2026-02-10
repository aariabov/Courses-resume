import { CheckOutlined } from "@ant-design/icons";
import { Badge, Button, Card, Col, Grid, Layout, Row, Skeleton, Typography } from "antd";
import React, { useEffect, useState } from "react";
import { Helmet } from "react-helmet";
import { PaymentType, PricesInfo } from "../../api/Api";
import { apiClient } from "../../api/ApiClient";
import { MARGIN } from "../../constants";
import userStore from "../../stores/UserStore";

const { Content } = Layout;
const { Title, Text } = Typography;
const { useBreakpoint } = Grid;

interface SubscriptionPlan {
  key: string;
  name: string;
  price?: number;
  periodText: string;
  paymentType: PaymentType;
  features: string[];
  highlight: boolean;
}

const SubscribePage: React.FC = () => {
  const [priceInfo, setPriceInfo] = useState<PricesInfo>();
  const [loading, setLoading] = useState(true);
  const paymentResultUrl = import.meta.env.VITE_PAYMENT_RESULT_URL;

  useEffect(() => {
    async function fetchPrice() {
      try {
        const priceResult = await apiClient.api.paymentGetPrices();
        setPriceInfo(priceResult.data.data);
      } catch (error) {
        console.error("Ошибка при получении цены:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchPrice();
  }, []);

  const plans: SubscriptionPlan[] = [
    {
      key: "monthly",
      name: "Месячная подписка",
      price: priceInfo?.perMonth,
      periodText: "руб/мес",
      paymentType: "PerMonth",
      features: [
        "Полный доступ к контенту",
        "Доступ ко всем заданиям",
        "Сохранение прогресса",
        "Обратная связь",
        "Безлимитное выполнение упражнений",
      ],
      highlight: false,
    },
    {
      key: "yearly",
      name: "Годовая подписка",
      price: priceInfo?.perYear,
      periodText: "руб/год",
      paymentType: "PerYear",
      features: [
        "Полный доступ к контенту",
        "Доступ ко всем заданиям",
        "Сохранение прогресса",
        "Обратная связь",
        "Безлимитное выполнение упражнений",
        `Выгода ${priceInfo ? Math.round((1 - priceInfo?.perYear / (priceInfo?.perMonth * 12)) * 100) : 0}%`,
      ],
      highlight: true,
    },
  ];

  const subscribe = async (type: PaymentType) => {
    try {
      if (!userStore.isLogged) {
        userStore.showLoginForm();
        return;
      }

      const createPaymentResult = await apiClient.api.paymentCreatePayment({
        type: type,
        returnUrl: paymentResultUrl,
      });
      const confirmationUrl = createPaymentResult.data.data;
      // ВАЖНО: не открывать в новой вкладке, ибо непонятно почему меняется deviceFingerprint
      window.location.href = confirmationUrl;
    } catch (error) {
      console.error("Ошибка при оформлении подписки:", error);
    } finally {
      //setLoading(false);
    }
  };

  return (
    <>
      <Helmet>
        <title>Подписка Premium — все возможности Devpull</title>
        <meta
          name="description"
          content="Оформите Premium подписку и пользуйтесь всеми возможностями сервиса без ограничений."
        />
      </Helmet>
      <Row justify="center">
        <Col style={{ display: "flex" }} span={24} xl={{ span: 18 }} xxl={{ span: 16 }}>
          <Content style={{ paddingLeft: MARGIN, paddingRight: MARGIN }}>
            <div style={{ textAlign: "center" }}>
              <Title level={2}>Premium подписка</Title>
              {/* <Text>Выберите план, который подходит именно вам</Text> */}

              <Row gutter={32} justify="center" style={{ marginTop: 40 }}>
                {plans.map((plan) => {
                  const card = (
                    <Card
                      bordered={false}
                      style={{
                        marginBottom: 20,
                        borderRadius: 16,
                        padding: 20,
                        background: plan.highlight ? "#f0f8ff" : "#fafafa",
                        border: plan.highlight ? "2px solid #1890ff" : "1px solid #e0e0e0",
                        boxShadow: plan.highlight ? "0 8px 24px rgba(24,144,255,0.2)" : "0 4px 12px rgba(0,0,0,0.05)",
                        transform: plan.highlight ? "scale(1)" : "scale(0.9)",
                        transition: "all 0.3s ease",
                      }}
                      hoverable
                    >
                      <Skeleton loading={loading} title active paragraph={{ rows: 11 }}>
                        <Title level={4} style={{ marginBottom: 10 }}>
                          {plan.name}
                        </Title>
                        <Title level={2} style={{ marginBottom: 0 }}>
                          {plan.price}
                          <Text type="secondary" style={{ fontSize: 16, marginLeft: 4 }}>
                            {plan.periodText}
                          </Text>
                        </Title>

                        <ul
                          style={{
                            textAlign: "left",
                            marginTop: 20,
                            paddingLeft: 0,
                            listStyle: "none",
                          }}
                        >
                          {plan.features.map((f, i) => (
                            <li key={i} style={{ marginBottom: 8 }}>
                              <CheckOutlined style={{ color: "#52c41a", marginRight: 8 }} />
                              <Text>{f}</Text>
                            </li>
                          ))}
                        </ul>

                        <Button
                          type={plan.highlight ? "primary" : "default"}
                          size="large"
                          onClick={() => subscribe(plan.paymentType)}
                          block
                          style={{
                            marginTop: 20,
                            borderRadius: 8,
                            fontWeight: "bold",
                            height: 50,
                          }}
                        >
                          {"Оформить подписку"}
                        </Button>
                      </Skeleton>
                    </Card>
                  );

                  return (
                    <Col xs={24} sm={12} md={10} key={plan.key}>
                      {plan.highlight ? (
                        <Badge.Ribbon text="Лучшее предложение" color="blue">
                          {card}
                        </Badge.Ribbon>
                      ) : (
                        card
                      )}
                    </Col>
                  );
                })}
              </Row>
            </div>
          </Content>
        </Col>
      </Row>
    </>
  );
};

export default SubscribePage;
