import { expect, test } from "@playwright/test";

test.use({
  viewport: {
    height: 1080,
    width: 1920,
  },
});

test.describe.configure({ mode: "serial" });

test("ошибка компиляции", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.click(".monaco-editor"); // фокус на редактор
  await page.getByRole("textbox", { name: "Editor content" }).press("ControlOrMeta+a");
  await page.keyboard.type('int number = "123";', { delay: 50 }); // печать по буквам

  await expect(page.locator(".squiggly-error").first()).toBeVisible();
  await page.getByText('"123"').first().hover();
  await expect(page.getByRole("tooltip")).toContainText("Cannot implicitly convert type 'string' to 'int'");
  await page.getByRole("img", { name: "play-circle" }).locator("svg").click();
  await expect(page.locator("pre")).toContainText(
    "Compiler error: (1,14): error CS0029: Cannot implicitly convert type 'string' to 'int'",
  );
});

test("вывод в консоль", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.click(".monaco-editor"); // фокус на редактор
  await page.getByRole("textbox", { name: "Editor content" }).press("ControlOrMeta+a");
  await page.keyboard.type('Console.Write("Привет!");', { delay: 50 }); // печать по буквам

  await page.getByRole("img", { name: "play-circle" }).locator("svg").click();
  await expect(page.locator("pre")).toContainText("Привет!");
});

test("ввод и вывод в консоль", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.click(".monaco-editor");
  await page.getByRole("textbox", { name: "Editor content" }).press("ControlOrMeta+a");
  await page.keyboard.type(
    'Console.Write("Введите имя: ");string input = Console.ReadLine();Console.Write($"Привет, {input}");',
    { delay: 50 },
  ); // печать по буквам

  await page.getByRole("img", { name: "play-circle" }).locator("svg").click();
  await expect(page.locator("pre")).toContainText("Введите имя: ");

  await page.locator("input").fill("Дима");
  await page.locator("input").press("Enter");
  await expect(page.locator("pre")).toContainText("Введите имя: ДимаПривет, Дима");
});

test("ввод и вывод в консоль на большом экране", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.getByRole("img", { name: "fullscreen" }).locator("svg").click();

  await page.click(".monaco-editor");
  await page.getByRole("textbox", { name: "Editor content" }).press("ControlOrMeta+a");
  await page.keyboard.type(
    'Console.Write("Введите имя: ");string input = Console.ReadLine();Console.Write($"Привет, {input}");',
    { delay: 50 },
  ); // печать по буквам

  await page.getByRole("img", { name: "play-circle" }).locator("svg").click();
  await expect(page.getByRole("dialog")).toContainText("Введите имя:");
  await expect(page.getByRole("dialog").locator("input")).toBeVisible();

  await page.getByRole("dialog").locator("input").fill("Дима");
  await page.getByRole("dialog").locator("input").press("Enter");
  await expect(page.getByRole("dialog")).toContainText("Введите имя: ДимаПривет, Дима");
});

test("ввод и вывод на маленьком экране, затем переключение на большой экран, аналогично, и обратно", async ({
  page,
}) => {
  // ввод на маленьком экране
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.click(".monaco-editor");
  await page.getByRole("textbox", { name: "Editor content" }).press("ControlOrMeta+a");
  await page.keyboard.type(
    'Console.Write("Введите имя: ");string input = Console.ReadLine();Console.Write($"Привет, {input}");',
    { delay: 50 },
  ); // печать по буквам

  // запуск кода на маленьком экране
  await page.getByRole("img", { name: "play-circle" }).locator("svg").click();
  await expect(page.locator("pre")).toContainText("Введите имя:");
  await expect(page.locator("input")).toBeVisible();
  await page.locator("input").click();

  // переключение на большой экран - состояние сохраняется
  await page.getByRole("img", { name: "fullscreen" }).locator("svg").click();
  await expect(page.getByRole("code")).toContainText(
    'Console.Write("Введите имя: ");string input = Console.ReadLine();Console.Write($"Привет, {input}");',
  );
  await expect(page.getByRole("dialog")).toContainText("Введите имя:");
  await expect(page.getByRole("dialog").locator("input")).toBeVisible();

  // ввод на большом экране
  await page.getByRole("dialog").locator("input").click();
  await page.getByRole("dialog").locator("input").fill("Дима");
  await page.getByRole("dialog").locator("input").press("Enter");
  await expect(page.getByRole("dialog")).toContainText("Введите имя: ДимаПривет, Дима");

  // переключение обратно на маленький экран
  await page.getByRole("img", { name: "fullscreen-exit" }).locator("svg").click();
  await expect(page.getByRole("code")).toContainText(
    'Console.Write("Введите имя: ");string input = Console.ReadLine();Console.Write($"Привет, {input}");',
  );
  await expect(page.locator("pre")).toContainText("Введите имя: ДимаПривет, Дима");
});

test("ошибка рантайм", async ({ page }) => {
  await page.goto("/courses/csharp-kids/about-c-sharp/code-execution");
  await page.click(".monaco-editor"); // фокус на редактор
  await page.getByRole("textbox", { name: "Editor content" }).press("ControlOrMeta+a");
  await page.keyboard.type(
    `int x = 10;
int y = 0;
int result = x / y;`,
    { delay: 50 },
  ); // печать по буквам

  await page.getByRole("img", { name: "play-circle" }).locator("svg").click();
  await expect(page.locator("pre")).toContainText("Attempted to divide by zero.");
});

test("крестики - нолики", async ({ page }) => {
  await page.goto("/courses/csharp-kids/completion/full-code");
  await page.getByRole("img", { name: "fullscreen" }).locator("svg").click();
  await page.getByRole("img", { name: "play-circle" }).locator("svg").click();

  // первый вывод, Юля делает ход: 0
  await expect(page.getByRole("dialog").locator("input")).toBeVisible();
  await expect(page.getByRole("dialog")).toContainText("|0| |1| |2| |3| |4| |5| |6| |7| |8|Игрок Юля делает ход:");
  await page.getByRole("dialog").locator("input").click();
  await page.getByRole("dialog").locator("input").fill("0");
  await page.getByRole("dialog").locator("input").press("Enter");

  // второй вывод, Дима делает ход: 3
  await expect(page.getByRole("dialog")).toContainText(
    "|0| |1| |2| |3| |4| |5| |6| |7| |8| Игрок Юля делает ход: 0 |Ю| |1| |2| |3| |4| |5| |6| |7| |8|Игрок Дима делает ход:",
  );
  await expect(page.getByRole("dialog").locator("input")).toBeVisible();
  await page.getByRole("dialog").locator("input").click();
  await page.getByRole("dialog").locator("input").fill("3");
  await page.getByRole("dialog").locator("input").press("Enter");

  // и так далее до победного
  await page.getByRole("dialog").locator("input").fill("1");
  await page.getByRole("dialog").locator("input").press("Enter");
  await page.getByRole("dialog").locator("input").fill("4");
  await page.getByRole("dialog").locator("input").press("Enter");
  await page.getByRole("dialog").locator("input").fill("2");
  await page.getByRole("dialog").locator("input").press("Enter");
  await expect(page.getByRole("dialog")).toContainText(
    "|0| |1| |2| |3| |4| |5| |6| |7| |8| Игрок Юля делает ход: 0 |Ю| |1| |2| |3| |4| |5| |6| |7| |8| Игрок Дима делает ход: 3 |Ю| |1| |2| |Д| |4| |5| |6| |7| |8| Игрок Юля делает ход: 1 |Ю| |Ю| |2| |Д| |4| |5| |6| |7| |8| Игрок Дима делает ход: 4 |Ю| |Ю| |2| |Д| |Д| |5| |6| |7| |8| Игрок Юля делает ход: 2 |Ю| |Ю| |Ю| |Д| |Д| |5| |6| |7| |8| Игрок Юля победил!!!Игра закончена!",
  );
});
