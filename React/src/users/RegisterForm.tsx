import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { Button, Form, Input } from "antd";
import React, { useState } from "react";
import { RegisterModel } from "../api/Api";
import { apiClient } from "../api/ApiClient";
import { nameof } from "../helpers/commonHelpers";

type RegisterFormProps = {
  showLoginForm: () => void;
  showConfirmForm: (email: string) => void;
};

const RegisterForm: React.FC<RegisterFormProps> = ({ showLoginForm, showConfirmForm }) => {
  const [form] = Form.useForm();
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (registerModel: RegisterModel) => {
    try {
      await form.validateFields();
      setIsLoading(true);

      const result = await apiClient.api.authRegister(registerModel);
      if (result.data.validationErrors) {
        const errors = result.data.validationErrors;
        form.setFields(Object.entries(errors).map(([key, errors]) => ({ name: key, errors: errors })));
      } else {
        showConfirmForm(registerModel.email);
      }
    } catch (error) {
      console.log("Validation failed:", error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Form<RegisterModel>
      form={form}
      onFinish={handleSubmit}
      name="login"
      initialValues={{ remember: true }}
      size="large"
    >
      <Form.Item<RegisterModel>
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
          onChange={() => form.setFields([{ name: nameof<RegisterModel>("email"), errors: undefined }])}
        />
      </Form.Item>
      <Form.Item<RegisterModel> name="password" rules={[{ required: true, message: "Пароль обязателен" }]}>
        <Input.Password
          prefix={<LockOutlined />}
          type="password"
          placeholder="пароль"
          onChange={() => form.setFields([{ name: nameof<RegisterModel>("password"), errors: undefined }])}
        />
      </Form.Item>
      <Form.Item<RegisterModel>
        name="confirmPassword"
        dependencies={["password"]}
        rules={[
          {
            required: true,
            message: "Подтвердите пароль",
          },
          ({ getFieldValue }) => ({
            validator(_, value) {
              if (!value || getFieldValue("password") === value) {
                return Promise.resolve();
              }
              return Promise.reject(new Error("Пароли не совпадают"));
            },
          }),
        ]}
      >
        <Input.Password
          prefix={<LockOutlined />}
          placeholder="повторите пароль"
          onChange={() => form.setFields([{ name: nameof<RegisterModel>("confirmPassword"), errors: undefined }])}
        />
      </Form.Item>
      <Button block type="primary" loading={isLoading} iconPosition="end" htmlType="submit">
        Регистрация
      </Button>
      <Button type="link" size="small" onClick={showLoginForm}>
        Вход
      </Button>
    </Form>
  );
};

export default RegisterForm;
