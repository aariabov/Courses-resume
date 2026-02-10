using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "payments",
                type: "text",
                nullable: false,
                defaultValue: "PerMonth"
            );

            migrationBuilder.Sql(
                @"
    UPDATE payments
    SET type = 'PerYear'
    WHERE amount = 1000
    "
            );

            migrationBuilder.Sql(
                @"ALTER TABLE payments 
      ALTER COLUMN type DROP DEFAULT"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "type", table: "payments");
        }
    }
}
