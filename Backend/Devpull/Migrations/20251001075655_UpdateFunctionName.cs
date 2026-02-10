using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFunctionName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("93dcf7ec-2367-4c93-a313-a0ac94629ee0"),
                column: "function_name",
                value: "Add");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("93dcf7ec-2367-4c93-a313-a0ac94629ee0"),
                column: "function_name",
                value: "");
        }
    }
}
