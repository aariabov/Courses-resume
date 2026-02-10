using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class InsertIsInRangeExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
insert into exercises (id, name, template, function_name, exercise_level_id)
values ('A3F7D2C1-9B84-4E6F-8C2A-5D1E3B7C9F20', 'Напишите функцию проверки попадания числа в диапазон: bool IsInRange(int val, int min, int max)', 'bool IsInRange(int val, int min, int max)
{
    // Ваш код
}', 'IsInRange', 1);"
            );

            migrationBuilder.Sql(
                @"
insert into exercise_lesson (exercises_id, lessons_id)
values ('A3F7D2C1-9B84-4E6F-8C2A-5D1E3B7C9F20', 'a42dc40d-8fee-4bd7-8d81-b4bf45b3f2de');"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
delete from exercise_lesson where exercises_id = 'A3F7D2C1-9B84-4E6F-8C2A-5D1E3B7C9F20';
delete from exercises where id = 'A3F7D2C1-9B84-4E6F-8C2A-5D1E3B7C9F20';"
            );
        }
    }
}
