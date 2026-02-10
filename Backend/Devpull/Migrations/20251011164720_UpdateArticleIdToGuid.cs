using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class UpdateArticleIdToGuid : Migration
    {
        private const string TableName = "articles";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(name: "pk_articles", table: TableName);

            // Rename existing id column to url
            migrationBuilder.RenameColumn(name: "id", table: TableName, newName: "url");

            // Add new id column with UUID type and generate values
            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: TableName,
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()"
            );

            // Add primary key constraint to id column
            migrationBuilder.AddPrimaryKey(name: "pk_articles", table: TableName, column: "id");

            // Add unique constraint to url column
            migrationBuilder.CreateIndex(
                name: "ix_articles_url",
                table: TableName,
                column: "url",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the new id column
            migrationBuilder.DropColumn(name: "id", table: TableName);

            // Rename url back to id
            migrationBuilder.RenameColumn(name: "url", table: TableName, newName: "id");
        }
    }
}
