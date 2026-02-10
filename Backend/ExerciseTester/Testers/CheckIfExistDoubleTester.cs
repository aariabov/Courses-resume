using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class CheckIfExistDoubleTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(arr) => {{{code} return CheckIfExistDouble(arr);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], bool>>(str, _options);
        var testCases = new List<CheckIfExistDoubleTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new CheckIfExistDoubleTestCase((bool)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { true, new[] {0, 0, 1, 2}, true };
        yield return new object[] { false, new[] {3,1,7,11}, false };
        yield return new object[] { false, Array.Empty<int>(), false };
        yield return new object[] { false, new[] {1}, false };
        yield return new object[] { true, new[] {2,4}, false };
        yield return new object[] { true, new[] {4,2}, false };
        yield return new object[] { true, new[] {0,0}, false };
        yield return new object[] { true, new[] {-2,-4}, false };
        yield return new object[] { true, new[] {3,6,1}, false };
        yield return new object[] { false, new[] {5,11}, false };
        yield return new object[] { true, new[] {7,14,28}, false };
        yield return new object[] { true, new[] {100,50}, false };
        yield return new object[] { false, new[] {int.MaxValue, int.MinValue}, false };
        yield return new object[] { false, new[] {1,3,5,11}, false };
        yield return new object[] { true, new[] {2,3,7,14}, false };
    }

    public static bool StandardFunction(int[] arr)
    {
        if (arr == null || arr.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < arr.Length; i++)
        {
            for (int j = i + 1; j < arr.Length; j++)
            {
                if (arr[i] * 2 == arr[j] || arr[j] * 2 == arr[i])
                {
                    return true;
                }
            }
        }
        return false;
    }
}

public class CheckIfExistDoubleTestCase(bool expected, int[] arr, bool isPublic)
    : TestCaseBase<bool>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
