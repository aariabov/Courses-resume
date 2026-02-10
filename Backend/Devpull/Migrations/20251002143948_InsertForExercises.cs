using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class InsertForExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
insert into exercises (id, name, template, function_name, exercise_level_id)
values ('C1F2E3D4-5B6A-4789-8C0D-1E2F3A4B5C6D', 'Напишите функцию возведения числа в степень: int Power(int number, int exponent)', 'int Power(int number, int exponent)
{
    // Ваш код
}', 'Power', 1),
       ('D2E3F4A5-B6C7-4890-9D1E-2F3A4B5C6D7E', 'Напишите функцию поиска количества четных элементов массива: int CountEven(int[] arr)', 'int CountEven(int[] arr)
{
    // Ваш код
}', 'CountEven', 1),
       ('E3F4A5B6-C7D8-4901-A2B3-4C5D6E7F8091', 'Напишите функцию поиска индекса элемента массива: int IndexOf(int[] arr, int target)', 'int IndexOf(int[] arr, int target)
{
    // Ваш код
}', 'IndexOf', 1),
       ('F4A5B6C7-D8E9-4012-B3C4-5D6E7F8091A2', 'Напишите функцию реверса массива: int[] ReverseArray(int[] arr)', 'int[] ReverseArray(int[] arr)
{
    // Ваш код
}', 'ReverseArray', 1);"
            );

            migrationBuilder.Sql(
                @"
insert into exercise_lesson (exercises_id, lessons_id)
values ('C1F2E3D4-5B6A-4789-8C0D-1E2F3A4B5C6D', '47016b22-27f2-4fda-a73c-a1bbefd5c33a'),
       ('D2E3F4A5-B6C7-4890-9D1E-2F3A4B5C6D7E', '47016b22-27f2-4fda-a73c-a1bbefd5c33a'),
       ('E3F4A5B6-C7D8-4901-A2B3-4C5D6E7F8091', '47016b22-27f2-4fda-a73c-a1bbefd5c33a'),
       ('F4A5B6C7-D8E9-4012-B3C4-5D6E7F8091A2', '47016b22-27f2-4fda-a73c-a1bbefd5c33a');"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
