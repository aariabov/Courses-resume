using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class InsertJoinAndSquareExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
insert into exercises (id, name, template, function_name, exercise_level_id)
values ('3BD2C881-06BD-4076-BEFD-D709EEF7971A', 'Напишите функцию соединения всех элементов массива в одну строку: string Join(int[] a)', 'string Join(int[] a)
{
    // Ваш код
}', 'Join', 1), 
    ('F6D68F44-CF38-4A47-8072-DEBFF2968C16', 'Напишите функцию возведения числа во вторую степень: int Square(int a)', 'int Square(int a)
{
    // Ваш код
}', 'Square', 1);"
            );

            migrationBuilder.Sql(
                @"
insert into exercise_lesson (exercises_id, lessons_id)
values ('3BD2C881-06BD-4076-BEFD-D709EEF7971A', '64b881f7-aa9d-480f-b483-aff1cd0c56c7'),
       ('F6D68F44-CF38-4A47-8072-DEBFF2968C16', '64b881f7-aa9d-480f-b483-aff1cd0c56c7'),
       ('93DCF7EC-2367-4C93-A313-A0AC94629EE0', '64b881f7-aa9d-480f-b483-aff1cd0c56c7'),
       ('6D5902C0-5456-4F19-B6DA-6D262FA1038D', '64b881f7-aa9d-480f-b483-aff1cd0c56c7');"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
