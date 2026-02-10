using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Devpull.Migrations;

public partial class AddOrdToStepsAndLessons : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ord",
            table: "steps",
            type: "integer",
            nullable: false
        );

        migrationBuilder.AddColumn<int>(
            name: "ord",
            table: "lessons",
            type: "integer",
            nullable: false
        );

        migrationBuilder.CreateIndex(
            name: "ix_steps_course_id_ord",
            table: "steps",
            columns: new[] { "course_id", "ord" },
            unique: true
        );

        migrationBuilder.CreateIndex(
            name: "ix_lessons_step_id_ord",
            table: "lessons",
            columns: new[] { "step_id", "ord" },
            unique: true
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "ix_steps_course_id_ord", table: "steps");

        migrationBuilder.DropIndex(name: "ix_lessons_step_id_ord", table: "lessons");

        migrationBuilder.DropColumn(name: "ord", table: "steps");

        migrationBuilder.DropColumn(name: "ord", table: "lessons");

        migrationBuilder.CreateIndex(
            name: "ix_steps_course_id",
            table: "steps",
            column: "course_id"
        );

        migrationBuilder.CreateIndex(
            name: "ix_lessons_step_id",
            table: "lessons",
            column: "step_id"
        );
    }
}
