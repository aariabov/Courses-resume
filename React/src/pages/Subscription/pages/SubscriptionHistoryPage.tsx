import { Card, Col, Collapse, Flex, Layout, Row, Space, theme, Typography } from "antd";
import { observer } from "mobx-react-lite";
import { SubscriptionStatus } from "../../../api/Api";
import { MARGIN } from "../../../constants";
import { formatDate } from "../../../helpers/commonHelpers";
import userStore from "../../../stores/UserStore";

const { Text, Title } = Typography;
const { Content } = Layout;
const { Panel } = Collapse;

const SubscriptionHistoryPage: React.FC = observer(() => {
  const { token } = theme.useToken();

  const getStatus = (status: SubscriptionStatus) => {
    if (status === "Active") {
      return "активна";
    }

    return status === "Expired" ? "истекла" : "ожидает";
  };

  return (
    <Row justify="center">
      <Col
        style={{ display: "flex" }}
        span={24}
        md={{ span: 18 }}
        lg={{ span: 14 }}
        xl={{ span: 12 }}
        xxl={{ span: 10 }}
      >
        <Content style={{ paddingLeft: MARGIN, paddingRight: MARGIN }}>
          <h1>История платежей</h1>
          <Space direction="vertical" size="small" style={{ display: "flex" }}>
            {userStore.subscriptions?.map((s) => (
              <Card key={s.endDate}>
                <Flex align="center">
                  <div style={{ flex: 1 }}>
                    <Text style={{ fontSize: token.fontSizeLG, display: "block" }}>
                      {s.type === "PerMonth" ? "Месячная " : "Годовая "}
                    </Text>
                    <Text type="secondary">{`${formatDate(s.startDate)} - ${formatDate(s.endDate)}`}</Text>
                    {s.status === "Active" ? (
                      <Text style={{ color: token.colorPrimaryText }}>{` (${getStatus(s.status)})`}</Text>
                    ) : (
                      <Text type="secondary">{` (${getStatus(s.status)})`}</Text>
                    )}
                  </div>
                  <Text
                    style={{
                      marginBottom: 0,
                      fontSize: token.fontSizeLG,
                      fontWeight: token.fontWeightStrong,
                      flex: "0 0 auto",
                      display: "block",
                    }}
                  >
                    {s.amount}
                    <Text type="secondary" style={{ marginLeft: 4 }}>
                      руб.
                    </Text>
                  </Text>
                </Flex>
              </Card>
            ))}
          </Space>
        </Content>
      </Col>
    </Row>
  );
});

export default SubscriptionHistoryPage;
