import { expect, test } from "@playwright/test";
import { randomString } from "../src/helpers/commonHelpers";

test.use({
  viewport: {
    height: 1080,
    width: 1920,
  },
});

test("success_logout_after_registration", async ({ page }) => {
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

  // logout
  await page.getByRole("menuitem", { name: "user" }).hover();
  await page.getByRole("menuitem", { name: "Выход" }).click();
  await expect(page.getByRole("dialog")).toContainText("Вы действительно хотите выйти?");
  await page.getByRole("button", { name: "Выйти" }).click();
  await expect(page.locator(".ant-message")).toContainText("Вы вышли");
  await expect(page.locator(".ant-modal")).toBeHidden();
  await expect(page.getByRole("menu")).toContainText("Вход");
});

test("success_logout_after_login", async ({ page }) => {
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

  // logout
  await page.getByRole("menuitem", { name: "user" }).hover();
  await page.getByRole("menuitem", { name: "Выход" }).click();
  await expect(page.getByRole("dialog")).toContainText("Вы действительно хотите выйти?");
  await page.getByRole("button", { name: "Выйти" }).click();
  await expect(page.locator(".ant-message")).toContainText("Вы вышли");
  await expect(page.locator(".ant-modal")).toBeHidden();
  await expect(page.getByRole("menu")).toContainText("Вход");

  // логин с правильным паролем
  await page.getByText("Вход").click();
  await page.getByRole("textbox", { name: "email" }).click();
  await page.getByRole("textbox", { name: "email" }).fill(email);
  await page.getByRole("textbox", { name: "пароль" }).click();
  await page.getByRole("textbox", { name: "пароль" }).fill("1");
  await page.getByRole("button", { name: "Войти" }).click();

  await expect(page.locator(".ant-message")).toContainText("Вы успешно вошли");
  await expect(page.locator(".ant-modal")).toBeHidden();

  // logout
  await page.getByRole("menuitem", { name: "user" }).hover();
  await page.getByRole("menuitem", { name: "Выход" }).click();
  await expect(page.getByRole("dialog")).toContainText("Вы действительно хотите выйти?");
  await page.getByRole("button", { name: "Выйти" }).click();
  await expect(page.locator(".ant-message")).toContainText("Вы вышли");
  await expect(page.locator(".ant-modal")).toBeHidden();
  await expect(page.getByRole("menu")).toContainText("Вход");
});
