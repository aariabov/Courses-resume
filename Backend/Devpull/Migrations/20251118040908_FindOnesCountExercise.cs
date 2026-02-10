using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class FindOnesCountExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
INSERT INTO public.exercises
(id, ""template"", exercise_level_id, description, short_name, url)
VALUES('22907441-5DAE-48DC-B4D1-23D31030910A', 'int FindOnesCount(int[] nums)
{
    // Ваш код
}', 1, 'Дан массив целых чисел `nums`. Необходимо написать функцию `int FindOnesCount(int[] nums)`, которая вернет количество единиц в массиве.', 'Найти количество единиц в массиве', 'find-ones-count');
"
            );

            migrationBuilder.Sql(
                @"
INSERT INTO public.exercise_examples
(id, ""input"", ""output"", explanation, exercise_id)
VALUES('a1b2c3d4-1111-4222-8333-abcdef123451', 'nums = [1, 1, 2, 3]', '2', 'В массиве две единицы по индексам 0 и 1', '22907441-5DAE-48DC-B4D1-23D31030910A'),
      ('b2c3d4e5-2222-4333-8444-bcdefa234562', 'nums = [0, 5, 7, 9]', '0', 'Единиц в массиве нет', '22907441-5DAE-48DC-B4D1-23D31030910A'),
      ('c3d4e5f6-3333-4444-8555-cdefab345673', 'nums = [1, 1, 1, 1]', '4', 'Все элементы массива равны единице', '22907441-5DAE-48DC-B4D1-23D31030910A');
"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
