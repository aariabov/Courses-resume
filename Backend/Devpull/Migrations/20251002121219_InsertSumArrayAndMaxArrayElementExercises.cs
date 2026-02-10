using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class InsertSumArrayAndMaxArrayElementExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
insert into exercises (id, name, template, function_name, exercise_level_id)
values ('B1A2C3D4-E5F6-47A8-90AB-CDEF12345678', 'Напишите функцию суммирования всех элементов массива: int SumArray(int[] a)', 'int SumArray(int[] a)
{
    // Ваш код
}', 'SumArray', 1),
       ('0A1B2C3D-4E5F-6071-8293-A4B5C6D7E8F9', 'Напишите функцию поиска максимального элемента массива: int MaxArrayElement(int[] a)', 'int MaxArrayElement(int[] a)
{
    // Ваш код
}', 'MaxArrayElement', 1);"
            );

            migrationBuilder.Sql(
                @"
insert into exercise_lesson (exercises_id, lessons_id)
values ('B1A2C3D4-E5F6-47A8-90AB-CDEF12345678', '47016b22-27f2-4fda-a73c-a1bbefd5c33a'),
       ('0A1B2C3D-4E5F-6071-8293-A4B5C6D7E8F9', '47016b22-27f2-4fda-a73c-a1bbefd5c33a');"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
delete from exercise_lesson where exercises_id in ('B1A2C3D4-E5F6-47A8-90AB-CDEF12345678','0A1B2C3D-4E5F-6071-8293-A4B5C6D7E8F9');
delete from exercises where id in ('B1A2C3D4-E5F6-47A8-90AB-CDEF12345678','0A1B2C3D-4E5F-6071-8293-A4B5C6D7E8F9');"
            );
        }
    }
}
