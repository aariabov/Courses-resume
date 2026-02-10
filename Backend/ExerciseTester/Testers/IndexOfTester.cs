using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class IndexOfTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(arr, target) => {{{code} return IndexOf(arr, target);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int, int>>(str, _options);
        var testCases = new List<IndexOfTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new IndexOfTestCase((int)pars[0], (int[])pars[1], (int)pars[2], (bool)pars[3]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr, testCase.Target), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        // public
        yield return new object[] { 0, new[] { 1 }, 1, true };
        yield return new object[] { -1, new[] { 1 }, 2, true };
        yield return new object[] { 2, new[] { 1, 2, 3 }, 3, true };
        yield return new object[] { -1, new[] { 1, 2, 3 }, 4, true };

        // hidden
        yield return new object[] { 1, new[] { -1, -2, -3 }, -2, false };
        yield return new object[] { 3, new[] { 0, 0, 0, 1 }, 1, false };
        yield return new object[] { -1, new[] { int.MaxValue }, int.MinValue, false };
        yield return new object[] { 0, new[] { int.MinValue, int.MaxValue }, int.MinValue, false };
    }

    public static int StandardFunction(int[] arr, int target)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            if (arr[i] == target)
            {
                return i;
            }
        }
        return -1;
    }
}

public class IndexOfTestCase(int expected, int[] arr, int target, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public int Target { get; } = target;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]", Target.ToString()];
}





