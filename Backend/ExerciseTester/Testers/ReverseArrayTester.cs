using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class ReverseArrayTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(arr) => {{{code} return ReverseArray(arr);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int[]>>(str, _options);
        var testCases = new List<ReverseArrayTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new ReverseArrayTestCase((int[])pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        // public
        yield return new object[] { Array.Empty<int>(), Array.Empty<int>(), true };
        yield return new object[] { new[] { 1 }, new[] { 1 }, true };
        yield return new object[] { new[] { 3, 2, 1 }, new[] { 1, 2, 3 }, true };
        yield return new object[] { new[] { -1, 0, 1 }, new[] { 1, 0, -1 }, true };

        // hidden
        yield return new object[] { new[] { 5, 5, 5 }, new[] { 5, 5, 5 }, false };
        yield return new object[] { new[] { int.MaxValue, int.MinValue }, new[] { int.MinValue, int.MaxValue }, false };
        yield return new object[] { new[] { 0, 0, 0, 0 }, new[] { 0, 0, 0, 0 }, false };
    }

    public static int[] StandardFunction(int[] arr)
    {
        var result = new int[arr.Length];
        for (var i = 0; i < arr.Length; i++)
        {
            result[i] = arr[arr.Length - 1 - i];
        }
        return result;
    }
}

public class ReverseArrayTestCase(int[] expected, int[] arr, bool isPublic)
    : TestCaseBase<int[]>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}





