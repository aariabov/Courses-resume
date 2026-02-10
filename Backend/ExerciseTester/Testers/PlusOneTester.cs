using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class PlusOneTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(digits) => {{{code} return PlusOne(digits);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int[]>>(str, _options);
        var testCases = new List<PlusOneTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new PlusOneTestCase((int[])pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { new[] {1,2,4}, new[] {1,2,3}, true };
        yield return new object[] { new[] {1,0,0,0}, new[] {9,9,9}, false };
        yield return new object[] { new[] {1}, new[] {0}, false };
        yield return new object[] { new[] {1,0}, new[] {9}, false };
        yield return new object[] { new[] {2,0}, new[] {1,9}, false };
        yield return new object[] { new[] {1,3,0}, new[] {1,2,9}, false };
        yield return new object[] { new[] {9,0,0}, new[] {8,9,9}, false };
        yield return new object[] { new[] {4,3,2,2}, new[] {4,3,2,1}, false };
        yield return new object[] { new[] {2,8,0}, new[] {2,7,9}, false };
        yield return new object[] { new[] {9,9,0}, new[] {9,8,9}, false };
        yield return new object[] { new[] {0,1,0}, new[] {0,0,9}, false };
        yield return new object[] { new[] {1,0,1}, new[] {1,0,0}, false };
        yield return new object[] { new[] {9,9,9}, new[] {9,9,8}, false };
        yield return new object[] { new[] {9,9,9,0}, new[] {9,9,8,9}, false };
        yield return new object[] { new[] {2,2,3}, new[] {2,2,2}, false };
    }

    public static int[] StandardFunction(int[] digits)
    {
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            if (digits[i] < 9)
            {
                digits[i]++;
                return digits;
            }

            digits[i] = 0;
        }

        int[] result = new int[digits.Length + 1];
        result[0] = 1;
        return result;
    }
}

public class PlusOneTestCase(int[] expected, int[] arr, bool isPublic)
    : TestCaseBase<int[]>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
