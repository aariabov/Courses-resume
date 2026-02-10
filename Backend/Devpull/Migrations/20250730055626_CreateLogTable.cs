using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class CreateLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // не добавляются ограничения NOT NULL и FK, т.к
            // 1. лог должен писаться всегда, а если не пройдет ограничение, то лога не будет вообще
            // 2. юзера можно удалить, а лог оставить
            // 3. быстродействие, проверка ограничений занимает время
            migrationBuilder.Sql(
                @"
CREATE TABLE logs (
    id SERIAL PRIMARY KEY,
    user_id text,
    request_id TEXT,
    elapsed_ms int,
    message TEXT,
    message_template TEXT,
    level VARCHAR(16),
    raise_date TIMESTAMPTZ,
    exception TEXT,
    properties JSONB
);"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE logs;");
        }
    }
}
