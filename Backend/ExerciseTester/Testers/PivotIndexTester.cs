using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class PivotIndexTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(nums) => {{{code} return PivotIndex2(nums);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int>>(str, _options);
        var testCases = new List<PivotIndexTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new PivotIndexTestCase((int)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { 3, new[] {1,7,3,6,5,6}, true };
        yield return new object[] { -1, new[] {1,2,3}, false };
        yield return new object[] { -1, Array.Empty<int>(), false };
        yield return new object[] { 0, new[] {0}, false };
        yield return new object[] { 2, new[] {1, -1, 0}, false };
        yield return new object[] { 1, new[] {2,1,-1,1,2}, false };
        yield return new object[] { 0, new[] {1,-1,1,-1,1}, false };
        yield return new object[] { 0, new[] {0,0,0,0}, false };
        yield return new object[] { 2, new[] {1,2,3,3}, false };
        yield return new object[] { -1, new[] {1,1,1,1,4}, false };
        yield return new object[] { 1, new[] {2,0,2}, false };
        yield return new object[] { 1, new[] {1,0,1}, false };
        yield return new object[] { 1, new[] {-1,-1,-1}, false };
        yield return new object[] { 2, new[] {1,2,1,2,1}, false };
        yield return new object[] { -1, new[] {3,3,5,-1,0}, false };
    }

    public static int StandardFunction(int[] nums)
    {
        int totalSum = 0;
        for (int i = 0; i < nums.Length; i++)
        {
            totalSum += nums[i];
        }

        int leftSum = 0;
        for (int i = 0; i < nums.Length; i++)
        {
            if (leftSum == totalSum - leftSum - nums[i])
            {
                return i;
            }
            leftSum += nums[i];
        }

        return -1;
    }
}

public class PivotIndexTestCase(int expected, int[] arr, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
