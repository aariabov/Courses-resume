import { expect, test } from "@playwright/test";
import { randomString } from "../src/helpers/commonHelpers";

test.use({
  viewport: {
    height: 1080,
    width: 1920,
  },
});

test("invalid_model_should_be_validation_errors", async ({ page }) => {
  await page.goto("/");
  await page.getByText("Вход").click();
  await page.getByRole("button", { name: "Войти" }).click();
  await expect(page.getByText("Email обязателен")).toBeVisible();
  await expect(page.getByText("Пароль обязателен")).toBeVisible();
});

test("user_not_found_should_be_validation_errors", async ({ page }) => {
  const email = `${randomString()}@mail.ru`;

  await page.goto("/");
  await page.getByText("Вход").click();
  await page.getByRole("textbox", { name: "email" }).click();
  await page.getByRole("textbox", { name: "email" }).fill(email);
  await page.getByRole("textbox", { name: "пароль", exact: true }).click();
  await page.getByRole("textbox", { name: "пароль", exact: true }).fill("1");
  await page.getByRole("button", { name: "Войти" }).click();
  await expect(page.getByText("Пользователя с таким Email не существует")).toBeVisible();
});

test("wrong_password_should_be_validation_errors", async ({ page }) => {
  const email = `${randomString()}@mail.ru`;

  // регистрация
  await page.goto("/");
  await page.getByText("Вход").click();
  await page.getByText("Регистрация").click();
  await page.getByRole("textbox", { name: "email" }).click();
  await page.getByRole("textbox", { name: "email" }).fill(email);
  await page.getByRole("textbox", { name: "пароль", exact: true }).click();
  await page.getByRole("textbox", { name: "пароль", exact: true }).fill("1");
  await page.getByRole("textbox", { name: "повторите пароль" }).click();
  await page.getByRole("textbox", { name: "повторите пароль" }).fill("1");
  await page.getByRole("button", { name: "Регистрация" }).click();

  // подтверждение кода
  await page.locator("#confirm_code > input").first().click();
  await page.locator("#confirm_code > input").first().fill("1");
  await page.locator("#confirm_code > input:nth-child(2)").fill("1");
  await page.locator("input:nth-child(3)").fill("1");
  await page.locator("input:nth-child(4)").fill("1");
  await page.locator("input:nth-child(5)").fill("1");
  await page.locator("input:nth-child(6)").fill("1");
  await page.getByRole("button", { name: "Отправить" }).click();

  await expect(page.locator(".ant-message")).toContainText("Вы успешно зарегистрировались");
  await expect(page.locator(".ant-modal")).toBeHidden();

  // выход
  await page.getByRole("menuitem", { name: "user" }).hover();
  await page.getByRole("menuitem", { name: "Выход" }).click();
  await page.getByRole("button", { name: "Выйти" }).click();

  // логин с неправильным паролем
  await page.getByText("Вход").click();
  await page.getByRole("textbox", { name: "email" }).click();
  await page.getByRole("textbox", { name: "email" }).fill(email);
  await page.getByRole("textbox", { name: "пароль" }).click();
  await page.getByRole("textbox", { name: "пароль" }).fill("4242");
  await page.getByRole("button", { name: "Войти" }).click();
  await expect(page.getByText("Неправильный пароль")).toBeVisible();
});

test("correct_password_should_be_success", async ({ page }) => {
  const email = `${randomString()}@mail.ru`;

  // регистрация
  await page.goto("/");
  await page.getByText("Вход").click();
  await page.getByText("Регистрация").click();
  await page.getByRole("textbox", { name: "email" }).click();
  await page.getByRole("textbox", { name: "email" }).fill(email);
  await page.getByRole("textbox", { name: "пароль", exact: true }).click();
  await page.getByRole("textbox", { name: "пароль", exact: true }).fill("1");
  await page.getByRole("textbox", { name: "повторите пароль" }).click();
  await page.getByRole("textbox", { name: "повторите пароль" }).fill("1");
  await page.getByRole("button", { name: "Регистрация" }).click();

  // подтверждение кода
  await page.locator("#confirm_code > input").first().click();
  await page.locator("#confirm_code > input").first().fill("1");
  await page.locator("#confirm_code > input:nth-child(2)").fill("1");
  await page.locator("input:nth-child(3)").fill("1");
  await page.locator("input:nth-child(4)").fill("1");
  await page.locator("input:nth-child(5)").fill("1");
  await page.locator("input:nth-child(6)").fill("1");
  await page.getByRole("button", { name: "Отправить" }).click();

  await expect(page.locator(".ant-message")).toContainText("Вы успешно зарегистрировались");
  await expect(page.locator(".ant-modal")).toBeHidden();

  // выход
  await page.getByRole("menuitem", { name: "user" }).hover();
  await page.getByRole("menuitem", { name: "Выход" }).click();
  await page.getByRole("button", { name: "Выйти" }).click();

  // логин с правильным паролем
  await page.getByText("Вход").click();
  await page.getByRole("textbox", { name: "email" }).click();
  await page.getByRole("textbox", { name: "email" }).fill(email);
  await page.getByRole("textbox", { name: "пароль" }).click();
  await page.getByRole("textbox", { name: "пароль" }).fill("1");
  await page.getByRole("button", { name: "Войти" }).click();

  await expect(page.locator(".ant-message")).toContainText("Вы успешно вошли");
  await expect(page.locator(".ant-modal")).toBeHidden();
});

test("логин, удаление токенов, логин", async ({ page }) => {
  const email = `${randomString()}@mail.ru`;

  // регистрация
  await page.goto("/");
  await page.getByText("Вход").click();
  await page.getByText("Регистрация").click();
  await page.getByRole("textbox", { name: "email" }).click();
  await page.getByRole("textbox", { name: "email" }).fill(email);
  await page.getByRole("textbox", { name: "пароль", exact: true }).click();
  await page.getByRole("textbox", { name: "пароль", exact: true }).fill("1");
  await page.getByRole("textbox", { name: "повторите пароль" }).click();
  await page.getByRole("textbox", { name: "повторите пароль" }).fill("1");
  await page.getByRole("button", { name: "Регистрация" }).click();

  // подтверждение кода
  await page.locator("#confirm_code > input").first().click();
  await page.locator("#confirm_code > input").first().fill("1");
  await page.locator("#confirm_code > input:nth-child(2)").fill("1");
  await page.locator("input:nth-child(3)").fill("1");
  await page.locator("input:nth-child(4)").fill("1");
  await page.locator("input:nth-child(5)").fill("1");
  await page.locator("input:nth-child(6)").fill("1");
  await page.getByRole("button", { name: "Отправить" }).click();

  await expect(page.locator(".ant-message")).toContainText("Вы успешно зарегистрировались");
  await expect(page.locator(".ant-modal")).toBeHidden();

  // удаление токенов
  await page.evaluate(() => window.localStorage.removeItem("token"));
  await page.evaluate(() => window.localStorage.removeItem("refreshToken"));

  // логин с правильным паролем
  await page.goto("/");
  await page.getByText("Вход").click();
  await page.getByRole("textbox", { name: "email" }).click();
  await page.getByRole("textbox", { name: "email" }).fill(email);
  await page.getByRole("textbox", { name: "пароль" }).click();
  await page.getByRole("textbox", { name: "пароль" }).fill("1");
  await page.getByRole("button", { name: "Войти" }).click();

  await expect(page.locator(".ant-message")).toContainText("Вы успешно вошли");
  await expect(page.locator(".ant-modal")).toBeHidden();
});
