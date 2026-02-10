import { Col, Grid, Layout, Row, Skeleton } from "antd";
import type { ItemType } from "antd/es/breadcrumb/Breadcrumb";
import React from "react";
import { Helmet } from "react-helmet";
import { MARGIN } from "../constants";
import Md from "./Md";
import { theme } from "antd";
import { formatDate } from "../helpers/commonHelpers";
import Breadcrumbs from "./Breadcrumbs";

const { Content } = Layout;
const { useBreakpoint } = Grid;

type ArticleProps = {
  title: string;
  description: string;
  content: string;
  loading: boolean;
  createDate?: string;
  breadcrumbs?: ItemType[];
};

const ArticleContent: React.FC<ArticleProps> = ({ title, description, content, loading, createDate, breadcrumbs }) => {
  const screens = useBreakpoint();
  const { token } = theme.useToken();

  return (
    <>
      <Helmet>
        <title>{title}</title>
        <meta name="description" content={description} />
      </Helmet>
      <Row justify="center">
        <Col style={{ display: "flex" }} span={24} xl={{ span: 18 }} xxl={{ span: 16 }}>
          <Content style={{ paddingLeft: MARGIN, paddingRight: MARGIN }}>
            <Skeleton loading={loading} title active paragraph={{ rows: 21 }}>
              <>
                <Breadcrumbs customItems={breadcrumbs} />
                <Md markdown={content} showToc={screens.lg} />
                {createDate && (
                  <div
                    style={{
                      textAlign: "right",
                      marginTop: 16,
                      color: token.colorTextTertiary,
                      fontSize: token.fontSizeSM,
                    }}
                  >
                    {formatDate(createDate)}
                  </div>
                )}
              </>
            </Skeleton>
          </Content>
        </Col>
      </Row>
    </>
  );
};

export default ArticleContent;
