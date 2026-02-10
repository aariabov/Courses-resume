using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberToExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "example", table: "exercises");

            migrationBuilder.DropColumn(name: "function_name", table: "exercises");

            migrationBuilder.DropColumn(name: "name", table: "exercises");

            migrationBuilder
                .AddColumn<int>(
                    name: "number",
                    table: "exercises",
                    type: "integer",
                    nullable: false,
                    defaultValue: 0
                )
                .Annotation(
                    "Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                );

            migrationBuilder.CreateIndex(
                name: "ix_exercises_number",
                table: "exercises",
                column: "number",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_exercises_number", table: "exercises");

            migrationBuilder.DropColumn(name: "number", table: "exercises");

            migrationBuilder.AddColumn<string>(
                name: "example",
                table: "exercises",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "function_name",
                table: "exercises",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "exercises",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("6d5902c0-5456-4f19-b6da-6d262fa1038d"),
                columns: new[] { "example", "function_name", "name" },
                values: new object[]
                {
                    "",
                    "Min",
                    "Напишите функцию нахождения минимума из двух чисел: int Add(int a, int b)"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("93dcf7ec-2367-4c93-a313-a0ac94629ee0"),
                columns: new[] { "example", "function_name", "name" },
                values: new object[]
                {
                    "",
                    "Add",
                    "Напишите функцию сложения двух чисел: int Add(int a, int b)"
                }
            );
        }
    }
}
