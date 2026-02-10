using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class SeedArraysLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Уроки для шага 2 "Перебор и поиск"
            migrationBuilder.InsertData(
                table: "lessons",
                columns: new[] { "id", "name", "exercise_id", "step_id", "ord" },
                values: new object[,]
                {
                    {
                        new Guid("83C315AA-C742-4EA7-A57F-FCB8E2F0695B"),
                        "Найти количество единиц в массиве",
                        new Guid("22907441-5DAE-48DC-B4D1-23D31030910A"),
                        new Guid("DED722F8-5819-49E8-9498-AADBA0738780"),
                        1
                    },
                    {
                        new Guid("9B32DA31-F5A8-44DB-B760-E35A7071EEB2"),
                        "Найти максимальное количество последовательно идущих единиц",
                        new Guid("335C244B-8F2A-4C9D-A55E-9B1A2C3D4E5F"),
                        new Guid("DED722F8-5819-49E8-9498-AADBA0738780"),
                        2
                    },
                    {
                        new Guid("7D016689-052F-4EAC-9954-BF1A35E02D42"),
                        "Сумма элементов массива",
                        new Guid("b1a2c3d4-e5f6-47a8-90ab-cdef12345678"),
                        new Guid("DED722F8-5819-49E8-9498-AADBA0738780"),
                        3
                    },
                    {
                        new Guid("0A68B900-F6BB-4F2B-B360-FB0637D0EC10"),
                        "Максимальный элемент массива",
                        new Guid("0a1b2c3d-4e5f-6071-8293-a4b5c6d7e8f9"),
                        new Guid("DED722F8-5819-49E8-9498-AADBA0738780"),
                        4
                    },
                    {
                        new Guid("A2BAA042-56D9-4B3C-9186-58D87468637F"),
                        "Подсчет четных элементов",
                        new Guid("d2e3f4a5-b6c7-4890-9d1e-2f3a4b5c6d7e"),
                        new Guid("DED722F8-5819-49E8-9498-AADBA0738780"),
                        5
                    },
                    {
                        new Guid("8DC7B60B-EC7A-4829-B3D6-D80EA2B42E69"),
                        "Поиск индекса элемента",
                        new Guid("e3f4a5b6-c7d8-4901-a2b3-4c5d6e7f8091"),
                        new Guid("DED722F8-5819-49E8-9498-AADBA0738780"),
                        6
                    }
                }
            );

            // Уроки для шага 3 "Создание и изменение массива"
            migrationBuilder.InsertData(
                table: "lessons",
                columns: new[] { "id", "name", "exercise_id", "step_id", "ord" },
                values: new object[,]
                {
                    {
                        new Guid("8B590068-8401-4C69-8064-A30298893B0F"),
                        "Продублировать нули в массиве",
                        new Guid("b1c2d3e4-5f60-4890-9bcd-1e2f3a4b5c6d"),
                        new Guid("96720E4D-19A6-4DDA-8E43-7A4AF2B5E4DF"),
                        1
                    },
                    {
                        new Guid("9A72C93C-883F-445C-926C-43866E691080"),
                        "Слияние массивов",
                        new Guid("c2d3e4f5-6071-49a1-abcd-2f3a4b5c6d7e"),
                        new Guid("96720E4D-19A6-4DDA-8E43-7A4AF2B5E4DF"),
                        2
                    },
                    {
                        new Guid("B8B8AA96-DF86-4E06-A6F2-E51AC06EB7AE"),
                        "Заменить все элементы на максимальный справа",
                        new Guid("0789ab1c-5c26-4ef6-1a23-7e8f90112233"),
                        new Guid("96720E4D-19A6-4DDA-8E43-7A4AF2B5E4DF"),
                        3
                    },
                    {
                        new Guid("1A2D2503-58DD-4603-A336-C7F307905E56"),
                        "Сдвинуть нули",
                        new Guid("089abc2d-6d37-4f07-2b34-8f9011223344"),
                        new Guid("96720E4D-19A6-4DDA-8E43-7A4AF2B5E4DF"),
                        4
                    }
                }
            );

            // Уроки для шага 4 "InPlace операции"
            migrationBuilder.InsertData(
                table: "lessons",
                columns: new[] { "id", "name", "exercise_id", "step_id", "ord" },
                values: new object[,]
                {
                    {
                        new Guid("8392148A-5DF0-4A9C-8D55-44CD1B689FDB"),
                        "Продублировать нули",
                        new Guid("c2d3e4f5-6071-49a1-abcd-2f3a4b5c6d7e"),
                        new Guid("D23D0B8C-F28E-40F0-B9A3-31A03C6923FA"),
                        1
                    },
                    {
                        new Guid("CD425DF6-5311-4DE7-8F1A-C410262451C2"),
                        "Удалить значения из массива со сдвигом",
                        new Guid("d3e4f5a6-7182-4ab2-bcde-3a4b5c6d7e8f"),
                        new Guid("D23D0B8C-F28E-40F0-B9A3-31A03C6923FA"),
                        2
                    },
                    {
                        new Guid("06BC646E-7BE7-42D4-A6CC-5388E9C309D3"),
                        "Удалить дубли элементов",
                        new Guid("e4f5a6b7-8293-4bc3-cdef-4b5c6d7e8f90"),
                        new Guid("D23D0B8C-F28E-40F0-B9A3-31A03C6923FA"),
                        3
                    },
                    {
                        new Guid("B8978C85-B23C-4400-B0EA-18B6D818BF59"),
                        "Повернуть массив",
                        new Guid("27F1E7A9-5D1E-4C8A-9DE1-6F8A12345678"),
                        new Guid("D23D0B8C-F28E-40F0-B9A3-31A03C6923FA"),
                        4
                    }
                }
            );

            // Уроки для шага 5 "Продвинутый уровень"
            migrationBuilder.InsertData(
                table: "lessons",
                columns: new[] { "id", "name", "url", "content", "exercise_id", "step_id", "ord" },
                values: new object[,]
                {
                    {
                        new Guid("FB630751-5192-4E0C-B765-C91CF4068C7D"),
                        "Существование элемента",
                        "check-double",
                        null,
                        new Guid("f5a6b7c8-93a4-4cd4-def0-5c6d7e8f9011"),
                        new Guid("675B33A9-2ABC-4CE7-B27A-34AE1F36DCA3"),
                        1
                    },
                    {
                        new Guid("255B032B-4C1A-40DC-847D-60C3C5F5EC74"),
                        "Является ли массив горным",
                        "valid-mountain-array",
                        null,
                        new Guid("06a7b8c9-4b15-4de5-0f12-6d7e8f901122"),
                        new Guid("675B33A9-2ABC-4CE7-B27A-34AE1F36DCA3"),
                        2
                    },
                    {
                        new Guid("101639D1-A34E-4EC9-9BEE-9CC89D61D15B"),
                        "Сколько детей стоит не на своем месте",
                        "height-checker",
                        null,
                        new Guid("10abc3d4-7e48-4108-3c45-901122334455"),
                        new Guid("675B33A9-2ABC-4CE7-B27A-34AE1F36DCA3"),
                        3
                    },
                    {
                        new Guid("9CBD913A-CABC-45A8-A630-374F3E7E14C7"),
                        "Найти 3ий максимальный элемент",
                        "third-max",
                        null,
                        new Guid("11bcd4e5-8f59-4219-4d56-112233445566"),
                        new Guid("675B33A9-2ABC-4CE7-B27A-34AE1F36DCA3"),
                        4
                    },
                    {
                        new Guid("188A246D-BE32-4AFB-80E0-9E0FEC105912"),
                        "Найти два числа в массиве",
                        "two-sum",
                        null,
                        null,
                        new Guid("675B33A9-2ABC-4CE7-B27A-34AE1F36DCA3"),
                        5
                    },
                    {
                        new Guid("609B9F14-AA3D-453C-9AD6-1FB5AA425A52"),
                        "Минимальная длина подмассива",
                        "min-subarray-len",
                        null,
                        null,
                        new Guid("675B33A9-2ABC-4CE7-B27A-34AE1F36DCA3"),
                        6
                    },
                    {
                        new Guid("A404733E-4E0C-4186-B95D-533447D5751D"),
                        "Итог",
                        "conclusion",
                        @"# Итог

Поздравляем! Вы успешно прошли курс по работе с массивами в C# и освоили множество важных концепций и техник.

Что вы изучили:
- **Основы массивов:** создание, чтение и запись элементов, работа с индексами, перебор и поиск.
- **Манипуляции с массивами:** изменение элементов, дублирование, сдвиги, объединение массивов.
- **InPlace операции:** эффективная модификация массивов без создания дополнительных структур.
- **Продвинутые алгоритмы:** сложный анализ и модификации массивов.
- **Ключевые техники:** циклы, обработка индексов, фильтрация и подсчет элементов, ротация массива, удаление дубликатов и значений, оптимизация алгоритмов для работы in-place.

Вы проделали отличную работу! Вы освоили ключевые концепции работы с массивами в C#, укрепили алгоритмическое мышление и научились решать задачи различной сложности.

Это важный шаг на пути к становлению профессиональным разработчиком. В других наших курсах по C# вы сможете углубить знания и изучить язык более детально.

Надеемся, что курс был полезным для вас. Продолжайте практиковать, закреплять навыки и развивать свои умения. Ждём вас на других наших курсах!",
                        null,
                        new Guid("675B33A9-2ABC-4CE7-B27A-34AE1F36DCA3"),
                        7
                    }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем уроки
            // Шаг 2
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("83C315AA-C742-4EA7-A57F-FCB8E2F0695B")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("9B32DA31-F5A8-44DB-B760-E35A7071EEB2")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("7D016689-052F-4EAC-9954-BF1A35E02D42")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("0A68B900-F6BB-4F2B-B360-FB0637D0EC10")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("A2BAA042-56D9-4B3C-9186-58D87468637F")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("8DC7B60B-EC7A-4829-B3D6-D80EA2B42E69")
            );
            // Шаг 3
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("8B590068-8401-4C69-8064-A30298893B0F")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("9A72C93C-883F-445C-926C-43866E691080")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("B8B8AA96-DF86-4E06-A6F2-E51AC06EB7AE")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("1A2D2503-58DD-4603-A336-C7F307905E56")
            );
            // Шаг 4
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("8392148A-5DF0-4A9C-8D55-44CD1B689FDB")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("CD425DF6-5311-4DE7-8F1A-C410262451C2")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("06BC646E-7BE7-42D4-A6CC-5388E9C309D3")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("B8978C85-B23C-4400-B0EA-18B6D818BF59")
            );
            // Шаг 5
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("FB630751-5192-4E0C-B765-C91CF4068C7D")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("255B032B-4C1A-40DC-847D-60C3C5F5EC74")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("101639D1-A34E-4EC9-9BEE-9CC89D61D15B")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("9CBD913A-CABC-45A8-A630-374F3E7E14C7")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("188A246D-BE32-4AFB-80E0-9E0FEC105912")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("609B9F14-AA3D-453C-9AD6-1FB5AA425A52")
            );
            migrationBuilder.DeleteData(
                "lessons",
                "id",
                new Guid("A404733E-4E0C-4186-B95D-533447D5751D")
            );
        }
    }
}
