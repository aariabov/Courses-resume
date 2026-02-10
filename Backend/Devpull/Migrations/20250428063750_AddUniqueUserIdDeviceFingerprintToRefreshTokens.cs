using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueUserIdDeviceFingerprintToRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "ix_refresh_tokens_user_id", table: "refresh_tokens");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id_device_fingerprint",
                table: "refresh_tokens",
                columns: new[] { "user_id", "device_fingerprint" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_user_id_device_fingerprint",
                table: "refresh_tokens"
            );

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id"
            );
        }
    }
}
