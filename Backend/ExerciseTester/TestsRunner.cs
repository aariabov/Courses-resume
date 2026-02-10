using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExerciseTester;

public static class TestsRunner
{
    public static FunctionTestingResult RunTests<T, TK>(Func<TK, T> func, List<TK> cases)
        where TK : TestCaseBase<T>
        where T : notnull
    {
        var results = new List<TestResult>();
        foreach (var testCase in cases)
        {
            TestResult testResult;
            try
            {
                var res = func(testCase);
                var isSuccess = AreEqual(res, testCase.Expected);
                Console.WriteLine(isSuccess);
                Console.WriteLine(res);
                if (!isSuccess)
                {
                    var resStr = FormatValue(res);
                    var expectedStr = FormatValue(testCase.Expected);
                    var error =
                        $"Результат выполнения функции: <{resStr}> не соответствует ожидаемому <{expectedStr}>.";

                    testResult = TestResult.CreateFailed(
                        testCase.Parameters,
                        testCase.IsPublic,
                        error
                    );
                }
                else
                {
                    testResult = TestResult.CreateSuccess(testCase.Parameters, testCase.IsPublic);
                }
            }
            catch (Exception e)
            {
                testResult = TestResult.CreateError(
                    testCase.Parameters,
                    testCase.IsPublic,
                    $"Произошла ошибка при выполнении: {e.Message}"
                );
            }

            results.Add(testResult);
        }

        return FunctionTestingResult.CreateTestResult(results.ToArray());
    }

    private static string FormatValue<T>(T value)
    {
        if (value is null)
            return "null";

        // Строка — отдельный случай, потому что это IEnumerable<char>
        if (value is string s)
            return $"\"{s}\"";

        // Если это словарь
        if (value is IDictionary dict)
        {
            var items = new List<string>();
            foreach (var key in dict.Keys)
            {
                var val = dict[key];
                items.Add($"{FormatValue(key)}: {FormatValue(val)}");
            }

            return "{ " + string.Join(", ", items) + " }";
        }

        // Если это обобщённая коллекция
        if (value is IEnumerable enumerable)
        {
            var items = new List<string>();
            foreach (var item in enumerable)
            {
                items.Add(FormatValue(item));
            }

            return "[" + string.Join(", ", items) + "]";
        }

        // Обычные типы
        return value.ToString()!;
    }

    private static bool AreEqual<T>(T a, T b)
    {
        // быстрые случаи
        if (ReferenceEquals(a, b))
        {
            return true;
        }
        if (a is null || b is null)
        {
            return false;
        }

        // массивы — структурное сравнение (работает для многомерных и вложенных массивов)
        if (a is Array arrA && b is Array arrB)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(arrA, arrB);
        }

        // любые IEnumerable (кроме string)
        if (a is IEnumerable enumA && b is IEnumerable enumB && a is not string && b is not string)
        {
            return enumA.Cast<object>().SequenceEqual(enumB.Cast<object>());
        }

        // fallback
        return EqualityComparer<T>.Default.Equals(a, b);
    }
}

public abstract class TestCaseBase<T>(T expected, bool isPublic)
{
    public T Expected { get; } = expected;
    public bool IsPublic { get; } = isPublic;
    public abstract string[] Parameters { get; }
}
