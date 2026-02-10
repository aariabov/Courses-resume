using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class IfElseExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
insert into exercises (id, name, template, function_name, exercise_level_id)
values ('9A1B2C3D-4E5F-6071-8293-A4B5C6D7E8F0', 'Напишите функцию: int MaxOfTwo(int a, int b)', 'int MaxOfTwo(int a, int b)
{
    // Ваш код
}', 'MaxOfTwo', 1);
"
            );

            migrationBuilder.Sql(
                @"
insert into exercise_lesson (exercises_id, lessons_id)
values ('9A1B2C3D-4E5F-6071-8293-A4B5C6D7E8F0', '5a10faa4-cc8d-4934-98ab-c44c1b256561');
"
            );

            migrationBuilder.Sql(
                @"
insert into exercises (id, name, template, function_name, exercise_level_id)
values ('1A2B3C4D-5E6F-7081-92A3-B4C5D6E7F809', 'Напишите функцию проверки нечётности: bool IsOdd(int number)', 'bool IsOdd(int number)
{
    // Ваш код
}', 'IsOdd', 1);
"
            );

            migrationBuilder.Sql(
                @"
insert into exercise_lesson (exercises_id, lessons_id)
values ('1A2B3C4D-5E6F-7081-92A3-B4C5D6E7F809', '5a10faa4-cc8d-4934-98ab-c44c1b256561');
"
            );

            migrationBuilder.Sql(
                @"
insert into exercises (id, name, template, function_name, exercise_level_id)
values ('2B3C4D5E-6F70-8192-A3B4-C5D6E7F8091A', 'Напишите функцию определения знака числа: string Sign(int number). Если число больше 0, верните ""положительное"", если меньше 0, верните ""отрицательное"", если равно 0, верните ""ноль""', 'string Sign(int number)
{
    // Ваш код
    // число больше 0, верните ""положительное""
    // если меньше 0, верните ""отрицательное""
    // если равно 0, верните ""ноль""
}', 'Sign', 1);
"
            );

            migrationBuilder.Sql(
                @"
insert into exercise_lesson (exercises_id, lessons_id)
values ('2B3C4D5E-6F70-8192-A3B4-C5D6E7F8091A', '5a10faa4-cc8d-4934-98ab-c44c1b256561');
"
            );

            migrationBuilder.Sql(
                @"
insert into exercises (id, name, template, function_name, exercise_level_id)
values ('3C4D5E6F-7081-92A3-B4C5-D6E7F8091A2B', 'Напишите функцию категории возраста: string AgeCategory(int age). Если возраст меньше 12, верните ""ребенок"", если больше или равно 12 и меньше 18, верните ""подросток"", если больше или равно 18 и меньше 65, верните ""взрослый"", иначе верните ""пенсионер""', 'string AgeCategory(int age)
{
    // Ваш код
    // возраст меньше 12, верните ""ребенок""
    // если больше или равно 12 и меньше 18, верните ""подросток""
    // если больше или равно 18 и меньше 65, верните ""взрослый""
    // иначе верните ""пенсионер""
}', 'AgeCategory', 1);
"
            );

            migrationBuilder.Sql(
                @"
insert into exercise_lesson (exercises_id, lessons_id)
values ('3C4D5E6F-7081-92A3-B4C5-D6E7F8091A2B', '5a10faa4-cc8d-4934-98ab-c44c1b256561');
"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"delete from exercise_lesson where exercises_id in (
    '9A1B2C3D-4E5F-6071-8293-A4B5C6D7E8F0',
    '1A2B3C4D-5E6F-7081-92A3-B4C5D6E7F809',
    '2B3C4D5E-6F70-8192-A3B4-C5D6E7F8091A',
    '3C4D5E6F-7081-92A3-B4C5-D6E7F8091A2B');"
            );

            migrationBuilder.Sql(
                @"delete from exercises where id in (
    '9A1B2C3D-4E5F-6071-8293-A4B5C6D7E8F0',
    '1A2B3C4D-5E6F-7081-92A3-B4C5D6E7F809',
    '2B3C4D5E-6F70-8192-A3B4-C5D6E7F8091A',
    '3C4D5E6F-7081-92A3-B4C5-D6E7F8091A2B');"
            );
        }
    }
}
