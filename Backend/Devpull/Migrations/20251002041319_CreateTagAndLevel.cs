using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class CreateTagAndLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercise_levels",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<int>(type: "integer", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        name = table.Column<string>(type: "text", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercise_levels", x => x.id);
                }
            );

            migrationBuilder.InsertData(
                table: "exercise_levels",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Легкий" },
                    { 2, "Средний" },
                    { 3, "Сложный" }
                }
            );

            migrationBuilder.AddColumn<int>(
                name: "exercise_level_id",
                table: "exercises",
                type: "integer",
                nullable: false,
                defaultValue: 1
            );

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        name = table.Column<string>(type: "text", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "exercise_tag",
                columns: table =>
                    new
                    {
                        exercises_id = table.Column<Guid>(type: "uuid", nullable: false),
                        tags_id = table.Column<Guid>(type: "uuid", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercise_tag", x => new { x.exercises_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_exercise_tag_exercises_exercises_id",
                        column: x => x.exercises_id,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_exercise_tag_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("93dcf7ec-2367-4c93-a313-a0ac94629ee0"),
                column: "exercise_level_id",
                value: 1
            );

            migrationBuilder.CreateIndex(
                name: "ix_exercises_exercise_level_id",
                table: "exercises",
                column: "exercise_level_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_exercise_tag_tags_id",
                table: "exercise_tag",
                column: "tags_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_exercises_exercise_levels_exercise_level_id",
                table: "exercises",
                column: "exercise_level_id",
                principalTable: "exercise_levels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_exercises_exercise_levels_exercise_level_id",
                table: "exercises"
            );

            migrationBuilder.DropTable(name: "exercise_levels");

            migrationBuilder.DropTable(name: "exercise_tag");

            migrationBuilder.DropTable(name: "tags");

            migrationBuilder.DropIndex(name: "ix_exercises_exercise_level_id", table: "exercises");

            migrationBuilder.DropColumn(name: "exercise_level_id", table: "exercises");
        }
    }
}
