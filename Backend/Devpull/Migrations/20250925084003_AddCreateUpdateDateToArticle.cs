using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateUpdateDateToArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "create_date",
                table: "articles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: DateTime.UtcNow
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "update_date",
                table: "articles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: DateTime.UtcNow
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "create_date", table: "articles");

            migrationBuilder.DropColumn(name: "update_date", table: "articles");
        }
    }
}
