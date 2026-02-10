using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devpull.Migrations
{
    /// <inheritdoc />
    public partial class SplitDescriptionAndExamples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "example",
                table: "exercises",
                type: "text",
                nullable: true
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("6d5902c0-5456-4f19-b6da-6d262fa1038d"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Даны два целых числа `a` и `b`. Необходимо написать функцию `int Min(int a, int b)`, которая вернет минимальное из них.",
                    @"Вход: a = 5, b = 3
Выход: 3
Объяснение: 3 меньше чем 5

Вход: a = -1, b = 2
Выход: -1
Объяснение: -1 меньше чем 2

Вход: a = 4, b = 4
Выход: 4
Объяснение: Числа равны, можно вернуть любое из них"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("93dcf7ec-2367-4c93-a313-a0ac94629ee0"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Даны два целых числа `a` и `b`. Необходимо написать функцию `int Add(int a, int b)`, которая вернет их сумму.",
                    @"Вход: a = 2, b = 3
Выход: 5
Объяснение: 2 + 3 = 5

Вход: a = -1, b = 1
Выход: 0
Объяснение: -1 + 1 = 0

Вход: a = 0, b = 0
Выход: 0
Объяснение: 0 + 0 = 0"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("3bd2c881-06bd-4076-befd-d709eef7971a"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дан массив целых чисел `a`. Необходимо написать функцию `string Join(int[] a)`, которая объединит все элементы массива в одну строку.",
                    @"Вход: a = [1, 2, 3]
Выход: ""123""
Объяснение: Все числа объединяются в одну строку без разделителей

Вход: a = [42]
Выход: ""42""
Объяснение: Массив из одного элемента преобразуется в строку

Вход: a = []
Выход: """"
Объяснение: Пустой массив преобразуется в пустую строку"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("f6d68f44-cf38-4a47-8072-debff2968c16"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дано целое число `a`. Необходимо написать функцию `int Square(int a)`, которая вернет квадрат этого числа.",
                    @"Вход: a = 5
Выход: 25
Объяснение: 5 * 5 = 25

Вход: a = -4
Выход: 16
Объяснение: -4 * -4 = 16

Вход: a = 0
Выход: 0
Объяснение: 0 * 0 = 0"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("2f4f6d5b-8e8c-4b5b-b3a7-2e6f3c9d1a11"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дано целое число `number`. Необходимо написать функцию `bool IsPositive(int number)`, которая вернет `true`, если число положительное, и `false` в противном случае.",
                    @"Вход: number = 5
Выход: true
Объяснение: 5 больше 0, значит это положительное число

Вход: number = -3
Выход: false
Объяснение: -3 меньше 0, значит это отрицательное число

Вход: number = 0
Выход: false
Объяснение: 0 не является положительным числом"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("c0e4c9d9-2d3b-4e1f-9e7b-1a2b3c4d5e6f"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дано целое число `number`. Необходимо написать функцию `bool IsEven(int number)`, которая вернет `true`, если число чётное, и `false`, если нечётное.",
                    @"Вход: number = 4
Выход: true
Объяснение: 4 делится на 2 без остатка

Вход: number = 7
Выход: false
Объяснение: 7 при делении на 2 дает остаток 1

Вход: number = 0
Выход: true
Объяснение: 0 считается чётным числом"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("a3f7d2c1-9b84-4e6f-8c2a-5d1e3b7c9f20"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Даны целые числа `val`, `min` и `max`. Необходимо написать функцию `bool IsInRange(int val, int min, int max)`, которая вернет `true`, если число `val` попадает в диапазон [`min`, `max`] включительно.",
                    @"Вход: val = 5, min = 1, max = 10
Выход: true
Объяснение: 5 находится между 1 и 10 включительно

Вход: val = 0, min = 1, max = 5
Выход: false
Объяснение: 0 меньше нижней границы диапазона

Вход: val = 3, min = 3, max = 3
Выход: true
Объяснение: Число равно обеим границам диапазона"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("b1a2c3d4-e5f6-47a8-90ab-cdef12345678"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дан массив целых чисел `a`. Необходимо написать функцию `int SumArray(int[] a)`, которая вернет сумму всех элементов массива.",
                    @"Вход: a = [1, 2, 3]
Выход: 6
Объяснение: 1 + 2 + 3 = 6

Вход: a = [-1, 0, 1]
Выход: 0
Объяснение: -1 + 0 + 1 = 0

Вход: a = []
Выход: 0
Объяснение: Сумма пустого массива равна 0"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("0a1b2c3d-4e5f-6071-8293-a4b5c6d7e8f9"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дан массив целых чисел `a`. Необходимо написать функцию `int MaxArrayElement(int[] a)`, которая вернет значение максимального элемента в массиве.",
                    @"Вход: a = [1, 3, 2]
Выход: 3
Объяснение: 3 - самое большое число в массиве

Вход: a = [-1, -2, -3]
Выход: -1
Объяснение: -1 больше чем -2 и -3

Вход: a = [42]
Выход: 42
Объяснение: В массиве из одного элемента он и будет максимальным"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("c1f2e3d4-5b6a-4789-8c0d-1e2f3a4b5c6d"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Даны целые числа `number` и `exponent`. Необходимо написать функцию `int Power(int number, int exponent)`, которая возведет число `number` в степень `exponent`.",
                    @"Вход: number = 2, exponent = 3
Выход: 8
Объяснение: 2 в степени 3 = 2 * 2 * 2 = 8

Вход: number = 5, exponent = 0
Выход: 1
Объяснение: Любое число в степени 0 равно 1

Вход: number = 2, exponent = 5
Выход: 32
Объяснение: 2 в степени 5 = 2 * 2 * 2 * 2 * 2 = 32"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("d2e3f4a5-b6c7-4890-9d1e-2f3a4b5c6d7e"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дан массив целых чисел `arr`. Необходимо написать функцию `int CountEven(int[] arr)`, которая подсчитает количество чётных чисел в массиве.",
                    @"Вход: arr = [1, 2, 3, 4, 5]
Выход: 2
Объяснение: В массиве два чётных числа: 2 и 4

Вход: arr = [2, 4, 6, 8]
Выход: 4
Объяснение: Все числа в массиве чётные

Вход: arr = [1, 3, 5]
Выход: 0
Объяснение: В массиве нет чётных чисел"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("e3f4a5b6-c7d8-4901-a2b3-4c5d6e7f8091"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дан массив целых чисел `arr` и целое число `target`. Необходимо написать функцию `int IndexOf(int[] arr, int target)`, которая вернет индекс первого вхождения числа `target` в массиве или -1, если число не найдено.",
                    @"Вход: arr = [1, 2, 3, 2], target = 2
Выход: 1
Объяснение: Число 2 впервые встречается в массиве под индексом 1

Вход: arr = [1, 2, 3], target = 4
Выход: -1
Объяснение: Число 4 не найдено в массиве

Вход: arr = [5], target = 5
Выход: 0
Объяснение: В массиве из одного элемента число 5 находится под индексом 0"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("f4a5b6c7-d8e9-4012-b3c4-5d6e7f8091a2"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дан массив целых чисел `arr`. Необходимо написать функцию `int[] ReverseArray(int[] arr)`, которая вернет новый массив с элементами в обратном порядке.",
                    @"Вход: arr = [1, 2, 3]
Выход: [3, 2, 1]
Объяснение: Элементы массива размещены в обратном порядке

Вход: arr = [1, 2]
Выход: [2, 1]
Объяснение: Два элемента меняются местами

Вход: arr = [5]
Выход: [5]
Объяснение: Массив из одного элемента остается без изменений"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("9a1b2c3d-4e5f-6071-8293-a4b5c6d7e8f0"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Даны два целых числа `a` и `b`. Необходимо написать функцию `int MaxOfTwo(int a, int b)`, которая вернет максимальное из них.",
                    @"Вход: a = 5, b = 3
Выход: 5
Объяснение: 5 больше чем 3

Вход: a = -1, b = 2
Выход: 2
Объяснение: 2 больше чем -1

Вход: a = 4, b = 4
Выход: 4
Объяснение: Числа равны, можно вернуть любое из них"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("1a2b3c4d-5e6f-7081-92a3-b4c5d6e7f809"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дано целое число `number`. Необходимо написать функцию `bool IsOdd(int number)`, которая вернет `true`, если число нечётное, и `false`, если чётное.",
                    @"Вход: number = 5
Выход: true
Объяснение: 5 при делении на 2 дает остаток 1

Вход: number = 4
Выход: false
Объяснение: 4 делится на 2 без остатка

Вход: number = 0
Выход: false
Объяснение: 0 считается чётным числом"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("2b3c4d5e-6f70-8192-a3b4-c5d6e7f8091a"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дано целое число `number`. Необходимо написать функцию `string Sign(int number)`, которая вернет строку, описывающую знак числа: \"положительное\" для положительных чисел, \"отрицательное\" для отрицательных и \"ноль\" для нуля.",
                    @"Вход: number = 42
Выход: ""положительное""
Объяснение: 42 больше 0

Вход: number = -15
Выход: ""отрицательное""
Объяснение: -15 меньше 0

Вход: number = 0
Выход: ""ноль""
Объяснение: Число равно 0"
                }
            );

            migrationBuilder.UpdateData(
                table: "exercises",
                keyColumn: "id",
                keyValue: new Guid("3c4d5e6f-7081-92a3-b4c5-d6e7f8091a2b"),
                columns: new[] { "description", "example" },
                values: new object[]
                {
                    "Дано целое число `age`. Необходимо написать функцию `string AgeCategory(int age)`, которая вернет возрастную категорию: \"ребенок\" для возраста до 12 лет, \"подросток\" от 12 до 17 лет, \"взрослый\" от 18 до 64 лет, \"пенсионер\" от 65 лет.",
                    @"Вход: age = 8
Выход: ""ребенок""
Объяснение: 8 лет меньше 12, это ребенок

Вход: age = 15
Выход: ""подросток""
Объяснение: 15 лет входит в диапазон 12-17 лет

Вход: age = 70
Выход: ""пенсионер""
Объяснение: 70 лет больше или равно 65, это пенсионер"
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "example", table: "exercises");
        }
    }
}
