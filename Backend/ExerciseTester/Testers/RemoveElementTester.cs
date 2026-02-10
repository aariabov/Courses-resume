using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class RemoveElementTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(nums, val) => {{{code} return RemoveElement(nums, val);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int, int>>(str, _options);
        var testCases = new List<RemoveElementTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new RemoveElementTestCase((int)pars[0], (int[])pars[1], (int)pars[2], (bool)pars[3]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr, testCase.Val), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { 2, new[] {3,2,2,3}, 3, true };
        yield return new object[] { 5, new[] {0,1,2,2,3,0,4,2}, 2, false };
        yield return new object[] { 0, Array.Empty<int>(), 1, false };
        yield return new object[] { 0, new[] {1}, 1, false };
        yield return new object[] { 1, new[] {1}, 2, false };
        yield return new object[] { 0, new[] {2,2,2}, 2, false };
        yield return new object[] { 3, new[] {2,3,4,5}, 3, false };
        yield return new object[] { 3, new[] {0,1,0,3,12}, 0, false };
        yield return new object[] { 2, new[] {4,5,6,4,4}, 4, false };
        yield return new object[] { 5, new[] {1,2,3,4,5}, 6, false };
        yield return new object[] { 0, new[] {1,1,1,1}, 1, false };
        yield return new object[] { 3, new[] {1,2,1,2,1,2}, 2, false };
        yield return new object[] { 2, new[] {int.MaxValue, int.MinValue, 1}, 1, false };
        yield return new object[] { 2, new[] {0,0,1,2,0}, 0, false };
        yield return new object[] { 2, new[] {7,7,7,8,9}, 7, false };
    }

    public static int StandardFunction(int[] nums, int val)
    {
        int k = 0;
        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[i] != val)
            {
                nums[k] = nums[i];
                k++;
            }
        }
        return k;
    }
}

public class RemoveElementTestCase(int expected, int[] arr, int val, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public int Val { get; } = val;
    public override string[] Parameters => new[] { $"[{string.Join(", ", Arr)}]", $"val={Val}" };
}
