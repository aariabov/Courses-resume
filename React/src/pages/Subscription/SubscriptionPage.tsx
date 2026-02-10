import { Button, Card, Col, Collapse, Layout, Result, Row, theme, Typography } from "antd";
import { observer } from "mobx-react-lite";
import { useState } from "react";
import { Link } from "react-router-dom";
import { SubscriptionInfo } from "../../api/Api";
import { MARGIN } from "../../constants";
import { formatDate, pluralizeDays } from "../../helpers/commonHelpers";
import userStore from "../../stores/UserStore";

const { Text, Title } = Typography;
const { Content } = Layout;
const { Panel } = Collapse;

const SubscriptionPage = observer(() => {
  const [data, setData] = useState<SubscriptionInfo[]>([]);
  const { token } = theme.useToken();

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
            {userStore.activeSubscription ? (
              <Card style={{ border: "none" }}>
                <Result
                  status="success"
                  title="Подписка успешно активирована!"
                  subTitle={
                    <>
                      Спасибо за оплату! Подписка действует еще{" "}
                      <Text strong>{userStore.activeSubscription.daysLeft}</Text>{" "}
                      {pluralizeDays(userStore.activeSubscription.daysLeft)} до{" "}
                      <Text strong>{formatDate(userStore.activeSubscription.endDate)}</Text>.
                    </>
                  }
                  extra={[
                    <Button type="primary" key="history" size="large">
                      <Link to="history">История платежей</Link>
                    </Button>,
                    <Button key="main" size="large">
                      <Link to="/">На главную</Link>
                    </Button>,
                  ]}
                />
              </Card>
            ) : (
              <Card style={{ border: "none" }}>
                <Result
                  status="warning"
                  title="У вас нет активной подписки."
                  subTitle={<>Оформите Premium подписку и пользуйтесь всеми возможностями сервиса без ограничений.</>}
                  extra={[
                    <Button type="primary" key="subscribe" size="large">
                      <Link to="/subscribe">Оформить</Link>
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
});

export default SubscriptionPage;
