using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class NewArrayExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
INSERT INTO public.exercises
(id, ""template"", exercise_level_id, description, short_name, url)
VALUES('27F1E7A9-5D1E-4C8A-9DE1-6F8A12345678', 'void Rotate(int[] nums, int k)
{
    // Ваш код
}', 3, 'Дан массив целых чисел `nums`. Необходимо написать функцию `void Rotate(int[] nums, int k)`, которая повернёт массив вправо на `k` шагов.', 'Поворот массива', 'rotate-array');
"
            );

            migrationBuilder.Sql(
                @"
INSERT INTO public.exercises
(id, ""template"", exercise_level_id, description, short_name, url)
VALUES('36E2F8B0-6E2F-4D9B-AFD2-7F9B23456789', 'int[] TwoSum(int[] numbers, int target)
{
    // Ваш код
}', 2, 'Дан отсортированный массив `numbers` и целевое значение `target`. Вернуть порядковые номера двух элементов в массиве так, что их сумма элементов равна `target`. Нумерация начинается с 1.', 'Сумма двух чисел', 'two-sum');
"
            );

            migrationBuilder.Sql(
                @"
INSERT INTO public.exercises
(id, ""template"", exercise_level_id, description, short_name, url)
VALUES('45F3C9C1-7F3C-4EAC-B0E3-8C0D34567890', 'int MinSubArrayLen(int target, int[] nums)
{
    // Ваш код
}', 2, 'Дан массив положительных целых `nums` и число `target`. Найти минимальную длину подмассива такого, что сумма его элементов >= `target`. Если нет такового, вернуть 0.', 'Минимальная длина подмассива', 'min-subarray-len');
"
            );

            migrationBuilder.Sql(
                @"
INSERT INTO public.exercise_examples
(id, ""input"", ""output"", explanation, exercise_id)
VALUES('934FE9D9-6703-4E23-87FF-1967A6E8816E', 'nums = [1, 2, 3, 4, 5], k = 2', '[3, 4, 5, 1, 2]', 'Повернуть массив вправо на 2 шага', '27F1E7A9-5D1E-4C8A-9DE1-6F8A12345678'),
      ('73A7CE13-029F-433A-BABA-9172437C60F1', 'nums = [1, 2, 3, 4], k = 1', '[4, 1, 2, 3]', 'Повернуть массив на 1 шаг', '27F1E7A9-5D1E-4C8A-9DE1-6F8A12345678');
"
            );

            migrationBuilder.Sql(
                @"
INSERT INTO public.exercise_examples
(id, ""input"", ""output"", explanation, exercise_id)
VALUES('60361D67-6671-4068-ABC0-3C5F96F79B64', 'numbers = [2, 7, 11, 15], target = 9', '[1, 2]', '2 + 7 = 9', '36E2F8B0-6E2F-4D9B-AFD2-7F9B23456789'),
      ('5A105A6F-D733-4AB6-BF62-D97A84959AFE', 'numbers = [1, 2, 3, 4], target = 3', '[1, 2]', '1 + 2 = 3', '36E2F8B0-6E2F-4D9B-AFD2-7F9B23456789');
"
            );

            migrationBuilder.Sql(
                @"
INSERT INTO public.exercise_examples
(id, ""input"", ""output"", explanation, exercise_id)
VALUES('F2096129-896C-4D1D-89C1-7A70EBAC731A', 'target = 7, nums = [2, 3, 1, 2, 4, 3]', '2', 'Минимальный подмассив с суммой >= 7 имеет длину 2: [4, 3] или [3, 4]', '45F3C9C1-7F3C-4EAC-B0E3-8C0D34567890'),
      ('02E04F1C-1F84-434E-BBC5-CE0A9F2975DE', 'target = 11, nums = [11]', '1', 'Один элемент равен 11', '45F3C9C1-7F3C-4EAC-B0E3-8C0D34567890');
"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
