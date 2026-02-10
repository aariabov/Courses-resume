import { UserOutlined } from "@ant-design/icons";
import { Button, Form, Input } from "antd";
import React, { useState } from "react";
import { ForgotPasswordModel } from "../api/Api";
import { apiClient } from "../api/ApiClient";
import { nameof } from "../helpers/commonHelpers";

type ForgotPasswordFormProps = {
  showLoginForm: () => void;
  showResetPasswordForm: (email: string) => void;
};

const ForgotPasswordForm: React.FC<ForgotPasswordFormProps> = ({ showLoginForm, showResetPasswordForm }) => {
  const [form] = Form.useForm();
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (forgotPasswordModel: ForgotPasswordModel) => {
    try {
      await form.validateFields();
      setIsLoading(true);

      const result = await apiClient.api.authForgotPassword(forgotPasswordModel);
      if (result.data.validationErrors) {
        const errors = result.data.validationErrors;
        form.setFields(Object.entries(errors).map(([key, errors]) => ({ name: key, errors: errors })));
      } else {
        showResetPasswordForm(forgotPasswordModel.email);
      }
    } catch (error) {
      console.log("Validation failed:", error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Form<ForgotPasswordModel> form={form} onFinish={handleSubmit} size="large">
      <Form.Item<ForgotPasswordModel>
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
          onChange={() => form.setFields([{ name: nameof<ForgotPasswordModel>("email"), errors: undefined }])}
        />
      </Form.Item>
      <Button block type="primary" loading={isLoading} iconPosition="end" htmlType="submit">
        Отправить код
      </Button>
      <Button type="link" size="small" onClick={showLoginForm}>
        Вход
      </Button>
    </Form>
  );
};

export default ForgotPasswordForm;
