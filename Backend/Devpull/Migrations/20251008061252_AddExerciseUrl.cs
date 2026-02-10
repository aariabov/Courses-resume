using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "url",
                table: "exercises",
                type: "text",
                nullable: true
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("6d5902c0-5456-4f19-b6da-6d262fa1038d"),
                column: "url",
                value: "min-of-two-numbers"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("93dcf7ec-2367-4c93-a313-a0ac94629ee0"),
                column: "url",
                value: "add-two-numbers"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("3bd2c881-06bd-4076-befd-d709eef7971a"),
                column: "url",
                value: "join-array"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("f6d68f44-cf38-4a47-8072-debff2968c16"),
                column: "url",
                value: "square-number"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("2f4f6d5b-8e8c-4b5b-b3a7-2e6f3c9d1a11"),
                column: "url",
                value: "is-positive"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("c0e4c9d9-2d3b-4e1f-9e7b-1a2b3c4d5e6f"),
                column: "url",
                value: "is-even"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("a3f7d2c1-9b84-4e6f-8c2a-5d1e3b7c9f20"),
                column: "url",
                value: "is-in-range"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("b1a2c3d4-e5f6-47a8-90ab-cdef12345678"),
                column: "url",
                value: "sum-array"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("0a1b2c3d-4e5f-6071-8293-a4b5c6d7e8f9"),
                column: "url",
                value: "max-array-element"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("c1f2e3d4-5b6a-4789-8c0d-1e2f3a4b5c6d"),
                column: "url",
                value: "power"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("d2e3f4a5-b6c7-4890-9d1e-2f3a4b5c6d7e"),
                column: "url",
                value: "count-even"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("e3f4a5b6-c7d8-4901-a2b3-4c5d6e7f8091"),
                column: "url",
                value: "index-of"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("f4a5b6c7-d8e9-4012-b3c4-5d6e7f8091a2"),
                column: "url",
                value: "reverse-array"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("9a1b2c3d-4e5f-6071-8293-a4b5c6d7e8f0"),
                column: "url",
                value: "max-of-two"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("1a2b3c4d-5e6f-7081-92a3-b4c5d6e7f809"),
                column: "url",
                value: "is-odd"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("2b3c4d5e-6f70-8192-a3b4-c5d6e7f8091a"),
                column: "url",
                value: "sign"
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("3c4d5e6f-7081-92a3-b4c5-d6e7f8091a2b"),
                column: "url",
                value: "age-category"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "url", table: "exercises");
        }
    }
}
