import { Button, Form, Input, Typography, message } from "antd";
import React, { useState } from "react";
import { ConfirmEmailModel } from "../api/Api";
import { apiClient } from "../api/ApiClient";
import userStore from "../stores/UserStore";

type ConfirmFormProps = {
  email: string;
  showRegisterForm: () => void;
  closeModal: () => void;
};

const ConfirmForm: React.FC<ConfirmFormProps> = ({ email, showRegisterForm, closeModal }) => {
  const [form] = Form.useForm();
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (model: ConfirmEmailModel) => {
    try {
      await form.validateFields();
      model.deviceFingerprint = userStore.deviceFingerprint;
      setIsLoading(true);

      const result = await apiClient.api.authConfirmEmail(model);
      if (result.data.validationErrors) {
        const errors = result.data.validationErrors;
        form.setFields(Object.entries(errors).map(([key, errors]) => ({ name: key, errors: errors })));
      } else {
        userStore.setTokens(result.data.data);
        userStore.loadUserInfo();
        closeModal();
        message.success("Вы успешно зарегистрировались");
      }
    } catch (error) {
      console.log("Validation failed:", error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Form<ConfirmEmailModel>
      form={form}
      onFinish={handleSubmit}
      name="confirm"
      initialValues={{ remember: true }}
      size="large"
    >
      <Form.Item>
        <Typography.Text>
          На email <b>{email}</b> отправлен 6-ти значный код.
        </Typography.Text>
      </Form.Item>
      <Form.Item<ConfirmEmailModel> name="email" initialValue={email} noStyle>
        <Input type="hidden" />
      </Form.Item>
      <Form.Item<ConfirmEmailModel> name="code" rules={[{ required: true, message: "Введите код" }]}>
        <Input.OTP length={6} />
      </Form.Item>

      <Button block type="primary" loading={isLoading} iconPosition="end" htmlType="submit">
        Отправить
      </Button>
      <Button type="link" size="small" onClick={showRegisterForm}>
        Регистрация
      </Button>
    </Form>
  );
};

export default ConfirmForm;
