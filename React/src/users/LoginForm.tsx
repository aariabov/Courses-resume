import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { Button, Checkbox, Flex, Form, Input, message } from "antd";
import React, { useState } from "react";
import { LoginModel } from "../api/Api";
import { apiClient } from "../api/ApiClient";
import { nameof } from "../helpers/commonHelpers";
import userStore from "../stores/UserStore";

type LoginFormProps = {
  showRegisterForm: () => void;
  showResetPasswordForm: () => void;
  closeModal: () => void;
};

const LoginForm: React.FC<LoginFormProps> = ({ showRegisterForm, showResetPasswordForm, closeModal }) => {
  const [form] = Form.useForm();
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (loginModel: LoginModel) => {
    try {
      await form.validateFields();
      setIsLoading(true);

      loginModel.deviceFingerprint = userStore.deviceFingerprint;
      const result = await apiClient.api.authLogin(loginModel);
      if (result.data.validationErrors) {
        const errors = result.data.validationErrors;
        form.setFields(Object.entries(errors).map(([key, errors]) => ({ name: key, errors: errors })));
      } else {
        userStore.setTokens(result.data.data);
        userStore.loadUserInfo();
        closeModal();
        message.success("Вы успешно вошли");
      }
    } catch (error) {
      console.log("Validation failed:", error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Form<LoginModel> form={form} onFinish={handleSubmit} name="login" size="large">
      <Form.Item<LoginModel>
        name="email"
        validateTrigger={["onBlur"]}
        rules={[
          { required: true, message: "Email обязателен" },
          { type: "email", message: "Некорректный формат Email" },
        ]}
      >
        <Input
          prefix={<UserOutlined />}
          placeholder="email"
          onChange={() => form.setFields([{ name: nameof<LoginModel>("email"), errors: undefined }])}
        />
      </Form.Item>
      <Form.Item<LoginModel> name="password" rules={[{ required: true, message: "Пароль обязателен" }]}>
        <Input
          prefix={<LockOutlined />}
          type="password"
          placeholder="пароль"
          onChange={() => form.setFields([{ name: nameof<LoginModel>("password"), errors: undefined }])}
        />
      </Form.Item>
      <Form.Item>
        <Flex justify="space-between" align="center">
          <Form.Item<LoginModel> name="rememberMe" initialValue={true} valuePropName="checked" noStyle>
            <Checkbox>Запомнить меня</Checkbox>
          </Form.Item>
          <Button type="link" size="small" onClick={showResetPasswordForm}>
            Забыли пароль?
          </Button>
        </Flex>
      </Form.Item>

      <Button block type="primary" loading={isLoading} iconPosition="end" htmlType="submit">
        Войти
      </Button>
      <Button type="link" size="small" onClick={showRegisterForm}>
        Регистрация
      </Button>
    </Form>
  );
};

export default LoginForm;
