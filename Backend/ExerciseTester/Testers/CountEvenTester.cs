using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class CountEvenTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(arr) => {{{code} return CountEven(arr);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int>>(str, _options);
        var testCases = new List<CountEvenTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new CountEvenTestCase((int)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        // public
        yield return new object[] { 0, Array.Empty<int>(), true };
        yield return new object[] { 1, new[] { 2 }, true };
        yield return new object[] { 0, new[] { 1 }, true };
        yield return new object[] { 2, new[] { 1, 2, 3, 4 }, true };

        // hidden
        yield return new object[] { 3, new[] { -2, 0, 2 }, false };
        yield return new object[] { 4, new[] { 2, 4, 6, 8 }, false };
        yield return new object[] { 0, new[] { -1, -3, -5 }, false };
        yield return new object[] { 1, new[] { int.MaxValue, int.MinValue }, false };
    }

    public static int StandardFunction(int[] arr)
    {
        var count = 0;
        for (var i = 0; i < arr.Length; i++)
        {
            if (arr[i] % 2 == 0)
            {
                count++;
            }
        }
        return count;
    }
}

public class CountEvenTestCase(int expected, int[] arr, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}





