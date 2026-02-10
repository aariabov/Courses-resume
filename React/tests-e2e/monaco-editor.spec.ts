import { test, expect } from "@playwright/test";
import { delay, randomString } from "../src/helpers/commonHelpers";

test.use({
  viewport: {
    height: 1080,
    width: 1920,
  },
});

test("подсветка ошибок при загрузке страницы", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/errors");
  // Дождаться появления контейнера редактора
  //await page.waitForSelector(".monaco-editor");

  // Дождаться появления курсора (чтобы быть уверенным, что Monaco инициализировался)
  //await page.waitForSelector(".monaco-editor .cursor");

  // Дополнительно: проверить, что редактор не пуст
  //   const text = await page.locator(".view-lines").first().textContent();
  //   expect(text).not.toBeNull();

  await expect(page.locator(".squiggly-error").first()).toBeVisible();
  await page.getByText('"123"').first().hover();
  await expect(page.getByRole("tooltip")).toContainText("Cannot implicitly convert type 'string' to 'int'");
});

test("подсветка ошибок при вводе кода", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.click(".monaco-editor"); // фокус на редактор
  await page.keyboard.type('int number = "123";', { delay: 50 }); // печать по буквам

  await expect(page.locator(".squiggly-error").first()).toBeVisible();
  await page.getByText('"123"').first().hover();
  await expect(page.getByRole("tooltip")).toContainText("Cannot implicitly convert type 'string' to 'int'");
});

test("подсветка нескольких ошибок при вводе кода", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.click(".monaco-editor"); // фокус на редактор
  await page.keyboard.type('int number = "123";Console.WriteLine(xz);', { delay: 50 }); // печать по буквам

  const errors = page.locator(".squiggly-error");

  await expect(errors.first()).toBeVisible();
  await page.getByText('"123"').first().hover();
  await expect(page.getByRole("tooltip")).toContainText("Cannot implicitly convert type 'string' to 'int'");

  await expect(errors.last()).toBeVisible();
  await page.getByText("xz").first().hover();
  await expect(page.getByRole("tooltip")).toContainText("The name 'xz' does not exist in the current context");
});

test("подсветка ошибок при переходе в полноэкранный режим", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/errors");
  await page.locator("pre").filter({ hasText: '1int number = "123' }).locator("svg").nth(1).click();

  await expect(page.locator(".squiggly-error").first()).toBeVisible();
  await page.getByRole("dialog").getByText('"123"').hover();
  await expect(page.getByRole("tooltip")).toContainText("Cannot implicitly convert type 'string' to 'int'");
});

test("подсветка ошибок при вводе кода в полноэкранном режиме", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.getByRole("img", { name: "fullscreen" }).locator("svg").click();

  await page.keyboard.type('int number = "123";', { delay: 50 }); // печать по буквам
  await expect(page.locator(".squiggly-error").first()).toBeVisible();
  await page.getByText('"123"').first().hover();
  await expect(page.getByRole("tooltip")).toContainText("Cannot implicitly convert type 'string' to 'int'");
});

test("intellisence тест", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.click(".monaco-editor"); // фокус на редактор

  await page.keyboard.type("Conso", { delay: 50 }); // печать по буквам
  await page.getByRole("option", { name: "Console", exact: true }).locator("a").click();

  await page.keyboard.type(".writ", { delay: 50 }); // печать по буквам
  await page.getByText("WriteWriteLineInsert (⏎)Show").click();

  await expect(page.locator("section")).toContainText('Console.WriteLine("Hello, World!");Console.WriteLine');
});

test("intellisence для полноэкранного режима", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.getByRole("img", { name: "fullscreen" }).locator("svg").click();

  await page.keyboard.type("Conso", { delay: 50 }); // печать по буквам
  await page.getByRole("option", { name: "Console", exact: true }).locator("a").click();

  await page.keyboard.type(".writ", { delay: 50 }); // печать по буквам
  await page.getByText("WriteWriteLineInsert (⏎)Show").click();

  await expect(page.locator("section")).toContainText('Console.WriteLine("Hello, World!");Console.WriteLine');
});
