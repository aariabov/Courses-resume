using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class FindOnesCountTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(nums) => {{{code} return FindOnesCount(nums);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int>>(str, _options);
        var testCases = new List<FindOnesCountTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new FindOnesCountTestCase((int)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        // public
        yield return new object[] { 0, Array.Empty<int>(), true };
        yield return new object[] { 1, new[] { 1 }, true };
        yield return new object[] { 0, new[] { 2 }, true };
        yield return new object[] { 2, new[] { 1, 2, 1, 3 }, true };
        yield return new object[] { 3, new[] { 1, 1, 1 }, true };

        // hidden
        yield return new object[] { 0, new[] { -1, 0, 2, 3 }, false };
        yield return new object[] { 4, new[] { 1, 1, 1, 1 }, false };
        yield return new object[] { 1, new[] { int.MaxValue, 1, int.MinValue }, false };
        yield return new object[] { 2, new[] { 5, 1, 7, 1, 9 }, false };
    }

    public static int StandardFunction(int[] nums)
    {
        int count = 0;
        foreach (int n in nums)
        {
            if (n == 1)
                count++;
        }
        return count;
    }
}

public class FindOnesCountTestCase(int expected, int[] arr, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
