using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class AddOldTokenToRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "old_token",
                table: "refresh_tokens",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_date",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "old_token", table: "refresh_tokens");

            migrationBuilder.DropColumn(name: "refresh_date", table: "refresh_tokens");
        }
    }
}
