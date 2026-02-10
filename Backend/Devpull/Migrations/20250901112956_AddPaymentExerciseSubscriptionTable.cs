using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentExerciseSubscriptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_user_id_device_fingerprint",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_user_id_device_fingerprint"
            );

            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        name = table.Column<string>(type: "text", nullable: false),
                        template = table.Column<string>(type: "text", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exercises", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table =>
                    new
                    {
                        id = table.Column<string>(type: "text", nullable: false),
                        user_id = table.Column<string>(type: "text", nullable: false),
                        amount = table.Column<decimal>(type: "numeric", nullable: false),
                        income_amount = table.Column<decimal>(type: "numeric", nullable: true),
                        confirmation_url = table.Column<string>(type: "text", nullable: false),
                        status = table.Column<string>(type: "text", nullable: false),
                        created_date = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        captured_date = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        create_json = table.Column<string>(type: "jsonb", nullable: false),
                        confirm_json = table.Column<string>(type: "text", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_payments_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "run_exercises",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        user_id = table.Column<string>(type: "text", nullable: false),
                        exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                        code = table.Column<string>(type: "text", nullable: false),
                        result = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                        date = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("pk_run_exercises", x => x.id);
                    table.ForeignKey(
                        name: "fk_run_exercises_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_run_exercises_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        user_id = table.Column<string>(type: "text", nullable: false),
                        payment_id = table.Column<string>(type: "text", nullable: false),
                        start_date = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        end_date = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscriptions_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_subscriptions_payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "exercises",
                columns: new[] { "id", "name", "template" },
                values: new object[]
                {
                    new Guid("93dcf7ec-2367-4c93-a313-a0ac94629ee0"),
                    "Напишите функцию сложения двух чисел: int Add(int a, int b)",
                    "int Add(int a, int b)\r\n{\r\n    // Ваш код\r\n}"
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_payments_user_id",
                table: "payments",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_run_exercises_exercise_id",
                table: "run_exercises",
                column: "exercise_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_run_exercises_user_id",
                table: "run_exercises",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_payment_id",
                table: "subscriptions",
                column: "payment_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "run_exercises");

            migrationBuilder.DropTable(name: "subscriptions");

            migrationBuilder.DropTable(name: "exercises");

            migrationBuilder.DropTable(name: "payments");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_user_id_device_fingerprint",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_user_id_device_fingerprint"
            );
        }
    }
}
