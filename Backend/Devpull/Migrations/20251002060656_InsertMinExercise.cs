using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class InsertMinExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("93dcf7ec-2367-4c93-a313-a0ac94629ee0"),
                column: "exercise_level_id",
                value: 1);

            migrationBuilder.InsertData(
                table: "exercises",
                columns: new[] { "id", "exercise_level_id", "function_name", "name", "template" },
                values: new object[] { new Guid("6d5902c0-5456-4f19-b6da-6d262fa1038d"), 1, "Min", "Напишите функцию нахождения минимума из двух чисел: int Add(int a, int b)", "int Min(int a, int b)\r\n{\r\n    // Ваш код\r\n}" });

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_exercise_levels_name",
                table: "exercise_levels",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tags_name",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_exercise_levels_name",
                table: "exercise_levels");

            migrationBuilder.DeleteData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("6d5902c0-5456-4f19-b6da-6d262fa1038d"));

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("93dcf7ec-2367-4c93-a313-a0ac94629ee0"),
                column: "exercise_level_id",
                value: 0);
        }
    }
}
