using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class FindMaxConsecutiveOnesExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
INSERT INTO public.exercises
(id, ""template"", exercise_level_id, description, short_name, url)
VALUES('335C244B-8F2A-4C9D-A55E-9B1A2C3D4E5F', 'int FindMaxConsecutiveOnes(int[] nums)
{
    // Ваш код
}', 1, 'Дан массив целых чисел `nums`. Необходимо написать функцию `int FindMaxConsecutiveOnes(int[] nums)`, которая вернет максимальное количество последовательно идущих единиц.', 'Найти максимальное количество последовательно идущих единиц', 'find-max-consecutive-ones');
"
            );

            migrationBuilder.Sql(
                @"
INSERT INTO public.exercise_examples
(id, ""input"", ""output"", explanation, exercise_id)
VALUES('d1e2f3a4-1111-4222-8333-abcdef123451', 'nums = [1, 1, 0, 1]', '2', 'Максимальная подряд идущая последовательность из двух единиц', '335C244B-8F2A-4C9D-A55E-9B1A2C3D4E5F'),
      ('d2e3f4a5-2222-4333-8444-bcdefa234562', 'nums = [0, 5, 7, 9]', '0', 'Единиц в массиве нет', '335C244B-8F2A-4C9D-A55E-9B1A2C3D4E5F'),
      ('d3f4a5b6-3333-4444-8555-cdefab345673', 'nums = [1, 1, 1, 1]', '4', 'Все элементы массива равны единице', '335C244B-8F2A-4C9D-A55E-9B1A2C3D4E5F');
"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
