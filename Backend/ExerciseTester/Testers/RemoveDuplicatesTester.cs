using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class RemoveDuplicatesTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(nums) => {{{code} return RemoveDuplicates(nums);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int>>(str, _options);
        var testCases = new List<RemoveDuplicatesTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new RemoveDuplicatesTestCase((int)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { 2, new[] {1,1,2}, true };
        yield return new object[] { 5, new[] {0,0,1,1,1,2,2,3,3,4}, false };
        yield return new object[] { 0, Array.Empty<int>(), false };
        yield return new object[] { 1, new[] {1}, false };
        yield return new object[] { 1, new[] {1,1,1,1}, false };
        yield return new object[] { 4, new[] {1,2,3,4}, false };
        yield return new object[] { 2, new[] {0,0,0,1}, false };
        yield return new object[] { 3, new[] {-3,-3,-2,-1}, false };
        yield return new object[] { 3, new[] {1,1,2,2,3,3}, false };
        yield return new object[] { 5, new[] {1,1,2,3,3,4,5}, false };
        yield return new object[] { 1, new[] {2,2}, false };
        yield return new object[] { 3, new[] {0,1,1,1,2}, false };
        yield return new object[] { 3, new[] {-1,0,0,1}, false };
        yield return new object[] { 2, new[] {int.MinValue, int.MinValue, int.MaxValue}, false };
        yield return new object[] { 5, new[] {1,2,2,3,4,4,5}, false };
    }

    public static int StandardFunction(int[] nums)
    {
        if (nums.Length == 0) return 0;
        int k = 1;
        for (int i = 1; i < nums.Length; i++)
        {
            if (nums[i] != nums[k - 1])
            {
                nums[k] = nums[i];
                k++;
            }
        }
        return k;
    }
}

public class RemoveDuplicatesTestCase(int expected, int[] arr, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
