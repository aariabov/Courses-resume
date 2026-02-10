using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class LessonNameNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "lessons",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.Sql(
                @"
    UPDATE lessons
    SET exercise_id = '36e2f8b0-6e2f-4d9b-afd2-7f9b23456789'
    WHERE id = '188a246d-be32-4afb-80e0-9e0fec105912';
    "
            );

            migrationBuilder.Sql(
                @"
    UPDATE lessons
    SET exercise_id = '45f3c9c1-7f3c-4eac-b0e3-8c0d34567890'
    WHERE id = '609b9f14-aa3d-453c-9ad6-1fb5aa425a52';
    "
            );

            migrationBuilder.Sql(
                @"
    UPDATE lessons
    SET name = null, url = null
    WHERE exercise_id is not null;
    "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "lessons",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );
        }
    }
}
