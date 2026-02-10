using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAppDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "lesson_step_id_fk", table: "lesson");

            migrationBuilder.DropForeignKey(name: "step_course_id_fk", table: "step");

            migrationBuilder.DropPrimaryKey(name: "step_pk", table: "step");

            migrationBuilder.DropPrimaryKey(name: "lesson_pk", table: "lesson");

            migrationBuilder.DropPrimaryKey(name: "course_pk", table: "course");

            migrationBuilder.RenameTable(name: "step", newName: "steps");

            migrationBuilder.RenameTable(name: "lesson", newName: "lessons");

            migrationBuilder.RenameTable(name: "course", newName: "courses");

            migrationBuilder.AddPrimaryKey(name: "pk_steps", table: "steps", column: "id");

            migrationBuilder.AddPrimaryKey(name: "pk_lessons", table: "lessons", column: "id");

            migrationBuilder.AddPrimaryKey(name: "pk_courses", table: "courses", column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_steps_step_id",
                table: "lessons",
                column: "step_id",
                principalTable: "steps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_steps_courses_course_id",
                table: "steps",
                column: "course_id",
                principalTable: "courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "fk_lessons_steps_step_id", table: "lessons");

            migrationBuilder.DropForeignKey(name: "fk_steps_courses_course_id", table: "steps");

            migrationBuilder.DropPrimaryKey(name: "pk_steps", table: "steps");

            migrationBuilder.DropPrimaryKey(name: "pk_lessons", table: "lessons");

            migrationBuilder.DropPrimaryKey(name: "pk_courses", table: "courses");

            migrationBuilder.RenameTable(name: "steps", newName: "step");

            migrationBuilder.RenameTable(name: "lessons", newName: "lesson");

            migrationBuilder.RenameTable(name: "courses", newName: "course");

            migrationBuilder.AddPrimaryKey(name: "step_pk", table: "step", column: "id");

            migrationBuilder.AddPrimaryKey(name: "lesson_pk", table: "lesson", column: "id");

            migrationBuilder.AddPrimaryKey(name: "course_pk", table: "course", column: "id");

            migrationBuilder.AddForeignKey(
                name: "lesson_step_id_fk",
                table: "lesson",
                column: "step_id",
                principalTable: "step",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "step_course_id_fk",
                table: "step",
                column: "course_id",
                principalTable: "course",
                principalColumn: "id"
            );
        }
    }
}
