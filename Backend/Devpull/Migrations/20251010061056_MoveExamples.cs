using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class MoveExamples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
WITH examples AS (
  SELECT
    e.id AS exercise_id,
    regexp_split_to_table(e.example, E'\\r?\\n\\s*\\r?\\n') AS block
  FROM exercises e
),
parsed AS (
  SELECT
    gen_random_uuid() AS id,
    exercise_id,
    trim(
      regexp_replace(block, E'(?s)^Вход:\\s*(.*?)\\r?\\nВыход:.*', E'\\1')
    ) AS input,
    trim(
      regexp_replace(block, E'(?s)^.*Выход:\\s*(.*?)\\r?\\nОбъяснение:.*', E'\\1')
    ) AS output,
    trim(
      regexp_replace(block, E'(?s)^.*Объяснение:\\s*(.*)$', E'\\1')
    ) AS explanation,
    block
  FROM examples
)
INSERT INTO exercise_examples (id, input, output, explanation, exercise_id)
SELECT id, input, output, explanation, exercise_id
FROM (
  SELECT
    gen_random_uuid() AS id,
    e.id AS exercise_id,
    trim(regexp_replace(block, E'(?s)^Вход:\\s*(.*?)\\r?\\nВыход:.*', E'\\1')) AS input,
    trim(regexp_replace(block, E'(?s)^.*Выход:\\s*(.*?)\\r?\\nОбъяснение:.*', E'\\1')) AS output,
    trim(regexp_replace(block, E'(?s)^.*Объяснение:\\s*(.*)$', E'\\1')) AS explanation
  FROM exercises e,
       regexp_split_to_table(e.example, E'\\r?\\n\\s*\\r?\\n') AS block
) parsed
WHERE input <> '' AND output <> '' AND explanation <> '';"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
