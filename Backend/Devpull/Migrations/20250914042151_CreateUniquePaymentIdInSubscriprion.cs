using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class CreateUniquePaymentIdInSubscriprion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_subscriptions_payment_id", table: "subscriptions");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_payment_id",
                table: "subscriptions",
                column: "payment_id",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_subscriptions_payment_id", table: "subscriptions");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_payment_id",
                table: "subscriptions",
                column: "payment_id"
            );
        }
    }
}
