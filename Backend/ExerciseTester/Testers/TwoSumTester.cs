using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class TwoSumTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(numbers, target) => {{{code} return TwoSum(numbers, target);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int, int[]>>(str, _options);
        var testCases = new List<TwoSumTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new TwoSumTestCase((int[])pars[0], (int[])pars[1], (int)pars[2], (bool)pars[3]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Numbers, testCase.Target), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        // public
        yield return new object[] { new[] { 1, 2 }, new[] { 2, 7, 11, 15 }, 9, true };
        yield return new object[] { new[] { 1, 2 }, new[] { 1, 2, 3, 4 }, 3, true };
        yield return new object[] { new[] { 1, 3 }, new[] { -1, 1, 2 }, 1, true };
        yield return new object[] { new[] { 1, 2 }, new[] { 2, 7, 11, 15 }, 9, true };
        yield return new object[] { new[] { 1, 3 }, new[] { 1, 3, 5, 7 }, 6, true };
        yield return new object[] { new[] { 2, 4 }, new[] { -5, 10, 2, 15 }, 25, true };
        yield return new object[] { new[] { 3, 4 }, new[] { 0, 2, 4, 6 }, 10, true };
        yield return new object[] { new[] { 2, 5 }, new[] { 1, 2, 4, 8, 16 }, 18, true };
        yield return new object[] { new[] { 1, 4 }, new[] { -3, 1, 4, 10 }, 7, true };
        yield return new object[] { new[] { 1, 5 }, new[] { 1, 2, 3, 4, 9 }, 10, true };
        yield return new object[] { new[] { 3, 5 }, new[] { -2, 0, 3, 5, 7 }, 10, true };
        yield return new object[] { new[] { 2, 3 }, new[] { 2, 3, 8, 20 }, 11, true };
        yield return new object[] { new int[] {}, new[] { 1, 2, 3, 4 }, 100, true };


        // hidden
        yield return new object[] { new[] { 2, 3 }, new[] { 2, 3, 4 }, 7, false };
        yield return new object[] { new[] { 1, 2 }, new[] { 0, 0 }, 0, false };
        yield return new object[] { new[] { 2, 5 }, new[] { -10, -3, 0, 7, 20 }, 17, false };
        yield return new object[] { new[] { 2, 3 }, new[] { 1, 4, 6, 8, 11 }, 10, false };
        yield return new object[] { new[] { 1, 5 }, new[] { -6, -2, 0, 1, 5 }, -1, false };
        yield return new object[] { new[] { 5, 6 }, new[] { 1, 2, 3, 4, 9, 13 }, 22, false };
        yield return new object[] { new[] { 3, 4 }, new[] { -8, -4, 5, 12 }, 17, false };
        yield return new object[] { new int[] {}, new[] { 10, 20, 30 }, 15, false };
        yield return new object[] { new[] { 2, 4 }, new[] { 1, 5, 7, 9 }, 14, false };
        yield return new object[] { new[] { 1, 6 }, new[] { -5, -2, 1, 3, 8, 10 }, 5, false };
        yield return new object[] { new[] { 3, 4 }, new[] { 0, 2, 4, 6, 9 }, 10, false };
        yield return new object[] { new[] { 1, 2 }, new[] { -100, 100, 200 }, 0, false };
    }

    public static int[] StandardFunction(int[] numbers, int target)
    {
        var i = 0;
        var j = numbers.Length - 1;
        while (i < j)
        {
            var sum = numbers[i] + numbers[j];
            if (sum == target)
            {
                return new int[] { i + 1, j + 1 };
            }

            if (sum < target)
            {
                i++;
            }
            else
            {
                j--;
            }
        }
        return [];
    }
}

public class TwoSumTestCase(int[] expected, int[] numbers, int target, bool isPublic)
    : TestCaseBase<int[]>(expected, isPublic)
{
    public int[] Numbers { get; } = numbers;
    public int Target { get; } = target;
    public override string[] Parameters => new[] { $"[{string.Join(", ", Numbers)}]", Target.ToString() };
}
