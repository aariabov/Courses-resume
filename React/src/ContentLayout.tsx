import { Col, Row } from "antd";
import type { ItemType } from "antd/es/breadcrumb/Breadcrumb";
import { Content } from "antd/es/layout/layout";
import { ReactNode } from "react";
import { MARGIN } from "./constants";
import MainPageMenu from "./pages/MainPage/MainPageMenu";
import Breadcrumbs from "./components/Breadcrumbs";

type ContentLayoutProps = {
  children: ReactNode;
  menu?: ReactNode;
  breadcrumbs?: ItemType[];
};

const ContentLayout: React.FC<ContentLayoutProps> = ({ children, menu, breadcrumbs }) => {
  return (
    <Row style={{ height: "100%" }}>
      <Col
        style={{ height: "100%", overflowY: "auto", marginTop: "1em", paddingLeft: "0.5em" }}
        offset={0}
        span={0}
        lg={{ span: 7 }}
        xl={{ offset: 3, span: 5 }}
        xxl={{ offset: 4, span: 4 }}
      >
        {menu ?? <MainPageMenu />}
      </Col>
      <Col lg={{ span: 17 }} xl={{ span: 16 }} style={{ height: "100%" }}>
        <Row style={{ width: "100%", height: "100%", overflowY: "auto" }}>
          <Col xl={{ span: 20 }} xxl={{ span: 18 }}>
            <Content
              style={{
                marginLeft: MARGIN,
                marginRight: MARGIN,
              }}
            >
              <Breadcrumbs customItems={breadcrumbs} />
              {children}
            </Content>
          </Col>
        </Row>
      </Col>
    </Row>
  );
};

export default ContentLayout;
