import { expect, test } from "@playwright/test";
import { delay, randomString } from "../src/helpers/commonHelpers";

const viewports = [
  { name: "mobile", size: { width: 375, height: 667 } },
  { name: "tablet", size: { width: 768, height: 1024 } },
  { name: "desktop", size: { width: 1440, height: 900 } },
];

// for (const vp of viewports) {
//   test.describe(`${vp.name} screen`, () => {
//     test.use({ viewport: vp.size });

//     test(`should render properly on ${vp.name}`, async ({ page }) => {
//       await page.goto("/");
//       await page.getByText("Вход").click();
//       await page.getByText("Регистрация").click();
//       await page.getByRole("button", { name: "Регистрация" }).click();
//       await expect(page.getByText("Email обязателен")).toBeVisible();
//       await expect(page.getByText("Пароль обязателен")).toBeVisible();
//       await expect(page.getByText("Подтвердите пароль")).toBeVisible();
//     });
//   });
// }

test.use({
  viewport: {
    height: 1080,
    width: 1920,
  },
});

test("invalid_model_should_be_validation_errors", async ({ page }) => {
  await page.goto("/");
  await page.getByText("Вход").click();
  await page.getByText("Регистрация").click();
  await page.getByRole("button", { name: "Регистрация" }).click();
  await expect(page.getByText("Email обязателен")).toBeVisible();
  await expect(page.getByText("Пароль обязателен")).toBeVisible();
  await expect(page.getByText("Подтвердите пароль")).toBeVisible();
});

test("invalid_code_should_be_validation_errors", async ({ page }) => {
  const email = `${randomString()}@mail.ru`;

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
  await expect(page.locator("#confirm")).toContainText(`На email ${email} отправлен 6-ти значный код.`);

  await page.locator("#confirm_code > input").first().click();
  await page.locator("#confirm_code > input").first().fill("0");
  await page.locator("#confirm_code > input:nth-child(2)").fill("0");
  await page.locator("input:nth-child(3)").fill("0");
  await page.locator("input:nth-child(4)").fill("0");
  await page.locator("input:nth-child(5)").fill("0");
  await page.locator("input:nth-child(6)").fill("0");
  await page.getByRole("button", { name: "Отправить" }).click();
  await expect(page.getByText("Неправильный код")).toBeVisible();
});

test("expired_code_should_be_validation_errors", async ({ page }) => {
  const email = `${randomString()}@mail.ru`;

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
  await expect(page.locator("#confirm")).toContainText(`На email ${email} отправлен 6-ти значный код.`);

  await delay(1000);
  await page.locator("#confirm_code > input").first().click();
  await page.locator("#confirm_code > input").first().fill("0");
  await page.locator("#confirm_code > input:nth-child(2)").fill("0");
  await page.locator("input:nth-child(3)").fill("0");
  await page.locator("input:nth-child(4)").fill("0");
  await page.locator("input:nth-child(5)").fill("0");
  await page.locator("input:nth-child(6)").fill("0");
  await page.getByRole("button", { name: "Отправить" }).click();
  await expect(page.getByText("Истек срок действия кода")).toBeVisible();
});

test("valid_code_should_be_success", async ({ page }) => {
  const email = `${randomString()}@mail.ru`;

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
});

test("not_confirm_user_in_db_valid_code_should_be_success", async ({ page }) => {
  const email = `${randomString()}@mail.ru`;

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
  await delay(100);
  await page.locator(".ant-modal-wrap").click(); // закрываем модалку с подтверждением кода

  await page.locator("#root").getByText("Вход").click();
  await page.getByText("Регистрация").click();
  await page.getByRole("textbox", { name: "email" }).click();
  await page.getByRole("textbox", { name: "email" }).fill(email);
  await page.getByRole("textbox", { name: "пароль", exact: true }).click();
  await page.getByRole("textbox", { name: "пароль", exact: true }).fill("1");
  await page.getByRole("textbox", { name: "повторите пароль" }).click();
  await page.getByRole("textbox", { name: "повторите пароль" }).fill("1");
  await page.getByRole("button", { name: "Регистрация" }).click();

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
});

test("user_exists_already_should_be_validation_errors", async ({ page }) => {
  const email = `${randomString()}@mail.ru`;

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

  await page.getByRole("menuitem", { name: "user" }).hover();
  await page.getByRole("menuitem", { name: "Выход" }).click();
  await page.getByRole("button", { name: "Выйти" }).click();
  await expect(page.locator(".ant-message")).toContainText("Вы вышли");

  await page.getByText("Вход").click();
  await page.getByText("Регистрация").click();
  await page.getByRole("textbox", { name: "email" }).click();
  await page.getByRole("textbox", { name: "email" }).fill(email);
  await page.getByRole("textbox", { name: "пароль", exact: true }).click();
  await page.getByRole("textbox", { name: "пароль", exact: true }).fill("1");
  await page.getByRole("textbox", { name: "повторите пароль" }).click();
  await page.getByRole("textbox", { name: "повторите пароль" }).fill("1");
  await page.getByRole("button", { name: "Регистрация" }).click();
  await expect(page.getByText("Пользователь уже существует, попробуйте Войти")).toBeVisible();
});
