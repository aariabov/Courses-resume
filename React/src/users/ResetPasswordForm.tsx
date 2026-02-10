import { LockOutlined } from "@ant-design/icons";
import { Button, Form, Input, message, Typography } from "antd";
import React, { useState } from "react";
import { RegisterModel, ResetPasswordModel } from "../api/Api";
import { apiClient } from "../api/ApiClient";
import { nameof } from "../helpers/commonHelpers";
import userStore from "../stores/UserStore";

type ResetPasswordFormProps = {
  email: string;
  showForgetPassportForm: () => void;
  closeModal: () => void;
};

const ResetPasswordForm: React.FC<ResetPasswordFormProps> = ({ email, showForgetPassportForm, closeModal }) => {
  const [form] = Form.useForm();
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (model: ResetPasswordModel) => {
    try {
      await form.validateFields();
      model.deviceFingerprint = userStore.deviceFingerprint;
      setIsLoading(true);

      const result = await apiClient.api.authResetPassword(model);
      if (result.data.validationErrors) {
        const errors = result.data.validationErrors;
        form.setFields(Object.entries(errors).map(([key, errors]) => ({ name: key, errors: errors })));
      } else {
        userStore.setTokens(result.data.data);
        closeModal();
        message.success("Вы успешно восстановили пароль");
      }
    } catch (error) {
      console.log("Validation failed:", error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Form<ResetPasswordModel>
      form={form}
      onFinish={handleSubmit}
      name="login"
      initialValues={{ remember: true }}
      size="large"
    >
      <Form.Item>
        <Typography.Text>
          На email <b>{email}</b> отправлен 6-ти значный код.
        </Typography.Text>
      </Form.Item>
      <Form.Item<ResetPasswordModel> name="email" initialValue={email} noStyle>
        <Input type="hidden" />
      </Form.Item>
      <Form.Item<ResetPasswordModel> name="code" rules={[{ required: true, message: "Введите код" }]}>
        <Input.OTP length={6} />
      </Form.Item>
      <Form.Item<ResetPasswordModel> name="password" rules={[{ required: true, message: "Пароль обязателен" }]}>
        <Input.Password
          prefix={<LockOutlined />}
          type="password"
          placeholder="пароль"
          onChange={() => form.setFields([{ name: nameof<RegisterModel>("password"), errors: undefined }])}
        />
      </Form.Item>
      <Form.Item<ResetPasswordModel>
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
        Восстановить пароль
      </Button>
      <Button type="link" size="small" onClick={showForgetPassportForm}>
        Изменить email
      </Button>
    </Form>
  );
};

export default ResetPasswordForm;
