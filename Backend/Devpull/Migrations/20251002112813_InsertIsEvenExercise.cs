using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class InsertIsEvenExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
insert into exercises (id, name, template, function_name, exercise_level_id)
values ('C0E4C9D9-2D3B-4E1F-9E7B-1A2B3C4D5E6F', 'Напишите функцию проверки чётности числа: bool IsEven(int number)', 'bool IsEven(int number)
{
    // Ваш код
}', 'IsEven', 1);"
            );

            migrationBuilder.Sql(
                @"
insert into exercise_lesson (exercises_id, lessons_id)
values ('C0E4C9D9-2D3B-4E1F-9E7B-1A2B3C4D5E6F', 'a42dc40d-8fee-4bd7-8d81-b4bf45b3f2de');"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
