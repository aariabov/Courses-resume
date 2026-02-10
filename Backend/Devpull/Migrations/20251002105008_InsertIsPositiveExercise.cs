using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class InsertIsPositiveExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
insert into exercises (id, name, template, function_name, exercise_level_id)
values ('2F4F6D5B-8E8C-4B5B-B3A7-2E6F3C9D1A11', 'Напишите функцию проверки положительности числа: bool IsPositive(int number)', 'bool IsPositive(int number)
{
    // Ваш код
}', 'IsPositive', 1);"
            );

            migrationBuilder.Sql(
                @"
insert into exercise_lesson (exercises_id, lessons_id)
values ('2F4F6D5B-8E8C-4B5B-B3A7-2E6F3C9D1A11', 'a42dc40d-8fee-4bd7-8d81-b4bf45b3f2de');"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
