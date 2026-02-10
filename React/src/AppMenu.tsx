import { CheckCircleFilled, LoginOutlined, UserOutlined } from "@ant-design/icons";
import { Avatar, Badge, Grid, Menu, MenuProps, message, Modal, theme } from "antd";
import { observer } from "mobx-react-lite";
import React, { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { apiClient } from "./api/ApiClient";
import { ArraysCourseUrl, CSharpKidsCourseUrl } from "./constants";
import { GetArticleUrl, GetCourseUrl, articlesUrl, exercisesUrl } from "./helpers/UrlHelper";
import userStore from "./stores/UserStore";
import ConfirmForm from "./users/ConfirmForm";
import ForgotPasswordForm from "./users/ForgotPasswordForm";
import LoginForm from "./users/LoginForm";
import RegisterForm from "./users/RegisterForm";
import ResetPasswordForm from "./users/ResetPasswordForm";

const { useBreakpoint } = Grid;

enum ShowForm {
  Login,
  Register,
  Confirm,
  ForgotPassword,
  ResetPassword,
}

const { confirm } = Modal;
const showLogoutConfirm = () => {
  confirm({
    title: "Вы действительно хотите выйти?",
    content: "После выхода станут недоступны некоторые функции.",
    okText: "Выйти",
    cancelText: "Отмена",
    async onOk() {
      try {
        await apiClient.api.authLogout({
          refreshToken: userStore.refreshToken!,
        });
      } catch (error) {
        console.log("Error", error);
      } finally {
        userStore.logout();
        message.success("Вы вышли");
      }
    },
  });
};

const AppMenu: React.FC = observer(() => {
  const { token } = theme.useToken();
  const location = useLocation();
  const screens = useBreakpoint();
  const [showForm, setShowForm] = useState<ShowForm>(ShowForm.Login);
  const [email, setEmail] = useState("");

  const items: MenuProps["items"] = [
    {
      key: "about",
      label: <Link to={GetArticleUrl("about")}>О сервисе</Link>,
    },
    {
      key: exercisesUrl,
      label: <Link to={`/${exercisesUrl}`}>Упражнения</Link>,
    },
    {
      key: CSharpKidsCourseUrl,
      label: <Link to={GetCourseUrl(CSharpKidsCourseUrl)}>Курс C# Kids</Link>,
    },
    {
      key: ArraysCourseUrl,
      label: <Link to={GetCourseUrl(ArraysCourseUrl)}>Курс Массивы C#</Link>,
    },
    {
      key: articlesUrl,
      label: <Link to={`/${articlesUrl}`}>Статьи</Link>,
    },
    {
      key: "subscribe",
      label: <Link to="subscribe">Подписка</Link>,
    },
    {
      key: "login",
      icon: <LoginOutlined />,
      label: "Вход",
      style: { marginLeft: "auto" },
    },
    {
      key: "user",
      icon: userStore.hasActiveSubscription ? (
        <Badge count={<CheckCircleFilled style={{ color: "#4096ff", fontSize: 12 }} />} offset={[-2, 10]}>
          <Avatar icon={<UserOutlined />} />
        </Badge>
      ) : (
        <Avatar icon={<UserOutlined />} />
      ),
      style: { marginLeft: "auto" },
      children: [
        { key: "email", label: userStore.userInfo?.email },
        { key: "subscription", label: <Link to="subscription">Моя подписка</Link> },
        { key: "logout", label: "Выход" },
      ],
    },
  ];

  let selectedKeys: string[] = [];
  if (location.pathname !== "/") {
    const itemKey = location.pathname.split("/")[1];
    selectedKeys = [itemKey];
  }

  const showRegisterForm = () => setShowForm(ShowForm.Register);
  const showForgotPasswordForm = () => setShowForm(ShowForm.ForgotPassword);
  const showLoginForm = () => setShowForm(ShowForm.Login);
  const showConfirmForm = (email: string) => {
    setShowForm(ShowForm.Confirm);
    setEmail(email);
  };
  const showResetPasswordForm = (email: string) => {
    setShowForm(ShowForm.ResetPassword);
    setEmail(email);
  };

  const getFormTitle = function (showForm?: ShowForm): string {
    if (showForm === ShowForm.Login) return "Вход";
    if (showForm === ShowForm.Register) return "Регистрация";
    if (showForm === ShowForm.Confirm) return "Подтверждение";
    if (showForm === ShowForm.ForgotPassword) return "Сброс пароля";
    if (showForm === ShowForm.ResetPassword) return "Восстановление пароля";
    return "";
  };

  const closeModal = () => {
    setShowForm(ShowForm.Login);
    userStore.hideLoginForm();
  };

  return (
    <>
      <Menu
        theme="dark"
        mode="horizontal"
        selectedKeys={selectedKeys}
        items={items.filter((item) => {
          if (item?.key === "login" && userStore.isLogged) {
            return false;
          }

          if (item?.key === "user" && !userStore.isLogged) {
            return false;
          }

          return true;
        })}
        onClick={({ key }) => {
          if (key === "login") {
            userStore.showLoginForm();
          }
          if (key === "logout") {
            showLogoutConfirm();
          }
        }}
        style={{
          flex: 1,
          minWidth: 0,
          justifyContent: screens.md ? "flex-start" : "flex-end",
          backgroundColor: token.Menu?.itemBg,
        }}
      />
      <Modal
        title={<div style={{ textAlign: "center" }}>{getFormTitle(showForm)}</div>}
        open={userStore.isLoginFormShow}
        footer={null}
        closable={false}
        onCancel={closeModal}
        style={{ maxWidth: "370px" }}
        width={"100%"}
        destroyOnClose
      >
        {showForm === ShowForm.Login && (
          <LoginForm
            showRegisterForm={showRegisterForm}
            showResetPasswordForm={showForgotPasswordForm}
            closeModal={closeModal}
          />
        )}
        {showForm === ShowForm.Register && (
          <RegisterForm showLoginForm={showLoginForm} showConfirmForm={showConfirmForm} />
        )}
        {showForm === ShowForm.Confirm && (
          <ConfirmForm email={email} showRegisterForm={showRegisterForm} closeModal={closeModal} />
        )}
        {showForm === ShowForm.ForgotPassword && (
          <ForgotPasswordForm showLoginForm={showLoginForm} showResetPasswordForm={showResetPasswordForm} />
        )}
        {showForm === ShowForm.ResetPassword && (
          <ResetPasswordForm email={email} showForgetPassportForm={showForgotPasswordForm} closeModal={closeModal} />
        )}
      </Modal>
    </>
  );
});

export default AppMenu;
