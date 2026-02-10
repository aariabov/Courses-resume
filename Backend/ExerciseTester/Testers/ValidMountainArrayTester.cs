using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class ValidMountainArrayTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(arr) => {{{code} return ValidMountainArray(arr);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], bool>>(str, _options);
        var testCases = new List<ValidMountainArrayTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new ValidMountainArrayTestCase((bool)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { false, Array.Empty<int>(), false };
        yield return new object[] { false, new[] {2,1}, false };
        yield return new object[] { true, new[] {0,3,2,1}, true };
        yield return new object[] { false, new[] {2,2,2}, false };
        yield return new object[] { false, new[] {0,1,2,3,4}, false };
        yield return new object[] { false, new[] {4,3,2,1,0}, false };
        yield return new object[] { true, new[] {0,2,1}, false };
        yield return new object[] { true, new[] {0,1,0}, false };
        yield return new object[] { true, new[] {0,1,2,1,0}, false };
        yield return new object[] { false, new[] {0,1,1,0}, false };
        yield return new object[] { true, new[] {1,3,2}, false };
        yield return new object[] { false, new[] {1,2}, false };
        yield return new object[] { false, new[] {3,5,5,2}, false };
        yield return new object[] { false, new[] {0,1,0,1,0}, false };
        yield return new object[] { true, new[] {0,2,3,4,5,3,1}, false };
    }

    public static bool StandardFunction(int[] arr)
    {
        int n = arr.Length;
        if (n < 3)
        {
            return false;
        }

        int i = 0;
        while (i + 1 < n && arr[i] < arr[i + 1])
        {
            i++;
        }

        if (i == 0 || i == n - 1)
        {
            return false;
        }

        while (i + 1 < n && arr[i] > arr[i + 1])
        {
            i++;
        }

        return i == n - 1;
    }
}

public class ValidMountainArrayTestCase(bool expected, int[] arr, bool isPublic)
    : TestCaseBase<bool>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
