using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class CreateLessonsXrefExercisesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise_lesson");
        }
    }
}
