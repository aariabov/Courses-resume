using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseIdToLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise_lesson");

            migrationBuilder.AddColumn<Guid>(
                name: "exercise_id",
                table: "lessons",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_lessons_exercise_id",
                table: "lessons",
                column: "exercise_id");

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_exercises_exercise_id",
                table: "lessons",
                column: "exercise_id",
                principalTable: "exercises",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_lessons_exercises_exercise_id",
                table: "lessons");

            migrationBuilder.DropIndex(
                name: "ix_lessons_exercise_id",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "exercise_id",
                table: "lessons");

            migrationBuilder.CreateTable(
                name: "exercise_lesson",
                columns: table => new
                {
                    exercises_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lessons_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercise_lesson", x => new { x.exercises_id, x.lessons_id });
                    table.ForeignKey(
                        name: "fk_exercise_lesson_exercises_exercises_id",
                        column: x => x.exercises_id,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_exercise_lesson_lessons_lessons_id",
                        column: x => x.lessons_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_exercise_lesson_lessons_id",
                table: "exercise_lesson",
                column: "lessons_id");
        }
    }
}
