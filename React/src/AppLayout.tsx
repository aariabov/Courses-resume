import { Col, Flex, Grid, Layout, Row, theme } from "antd";
import { Content } from "antd/es/layout/layout";
import React, { useEffect } from "react";
import { Link, Outlet, useLocation } from "react-router-dom";
import AppMenu from "./AppMenu";
import { setupMonacoProviders } from "./components/MonacoSetup";
import { ArraysCourseUrl, CSharpKidsCourseUrl, MARGIN } from "./constants";
import userStore from "./stores/UserStore";
import { exercisesUrl } from "./helpers/UrlHelper";

const { Header, Footer } = Layout;
const { useBreakpoint } = Grid;

const layoutStyle: React.CSSProperties = {
  minHeight: "100vh",
};

const courseLayoutStyle: React.CSSProperties = {
  height: "100vh",
  overflow: "hidden",
};

const AppLayout: React.FC = () => {
  const location = useLocation();
  const { token } = theme.useToken();
  const screens = useBreakpoint();

  useEffect(() => {
    setupMonacoProviders(); // Регистрируем один раз при запуске
    if (userStore.refreshToken) {
      userStore.loadUserInfo();
    }
  }, []);

  let isCourse = false;
  let isFullWidth = false;
  const pattern = new RegExp(`^/${exercisesUrl}/[^/]+$`);
  const courses = [CSharpKidsCourseUrl, ArraysCourseUrl];
  if (courses.some((course) => location.pathname.includes(course)) || pattern.test(location.pathname)) {
    isCourse = true;

    // если урок
    if (location.pathname.split("/").length === 5 || pattern.test(location.pathname)) {
      isFullWidth = true;
    }
  }

  let isFooterVertical = false;
  if (screens.xs) {
    isFooterVertical = true;
  }

  return (
    <Layout style={isCourse ? courseLayoutStyle : layoutStyle}>
      <Header style={{ padding: 0 }}>
        <Row>
          <Col
            offset={0}
            span={7}
            xl={{ offset: isFullWidth ? 0 : 3, span: isFullWidth ? 8 : 5 }}
            xxl={{ offset: isFullWidth ? 0 : 4, span: isFullWidth ? 8 : 4 }}
          >
            <Link
              to="/"
              style={{
                color: "white",
                fontSize: "26px",
                display: "flex",
                marginLeft: MARGIN,
              }}
            >
              Devpull
            </Link>
          </Col>
          <Col
            style={{ display: "flex" }}
            span={17}
            lg={{ offset: 1, span: 16 }}
            xl={{ offset: 1, span: 12 }}
            xxl={{ offset: 1, span: 11 }}
          >
            <AppMenu />
          </Col>
        </Row>
      </Header>
      <Content style={{ paddingBottom: token.padding }}>
        <Outlet />
      </Content>
      {isCourse ? null : (
        <Footer style={{ textAlign: "center" }}>
          Devpull.courses, {new Date().getFullYear()}. Все права защищены. Рябов Андрей Александрович, ИНН 590811359401.
          Email ryab777@mail.ru
          <br></br>
          <Flex vertical={isFooterVertical} justify="center" gap={isFooterVertical ? 0 : token.marginXXL}>
            <Link to="/offer">Публичная оферта</Link>
            <Link to="/privacy-policy">Политика конфиденциальности</Link>
            <Link to="https://t.me/devpull_bot" target="_blank">
              Написать в поддержку
            </Link>
          </Flex>
        </Footer>
      )}
    </Layout>
  );
};

export default AppLayout;
