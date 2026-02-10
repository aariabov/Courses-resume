using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class ThirdMaxTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(nums) => {{{code} return ThirdMax(nums);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int>>(str, _options);
        var testCases = new List<ThirdMaxTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new ThirdMaxTestCase((int)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { 1, new[] {3,2,1}, true };
        yield return new object[] { 2, new[] {1,2}, false };
        yield return new object[] { 1, new[] {2,2,3,1}, false };
        yield return new object[] { 1, new[] {1}, false };
        yield return new object[] { 2, new[] {2,2,2}, false };
        yield return new object[] { 3, new[] {5,4,3,2,1}, false };
        yield return new object[] { 3, new[] {5,5,4,4,3,3}, false };
        yield return new object[] { 2, new[] {1,2,3,4}, false };
        yield return new object[] { 3, new[] {7,1,5,3,5}, false };
        yield return new object[] { int.MinValue, new[] {int.MinValue, int.MinValue, -1, -2}, false };
        yield return new object[] { 10, new[] {10,9}, false };
        yield return new object[] { 1, new[] {3,2,2,1}, false };
        yield return new object[] { 25, new[] {100,50,50,25,100}, false };
        yield return new object[] { 3, new[] {2,3,1,4,5}, false };
        yield return new object[] { 3, new[] {5,4,4,3,2,1,1}, false };
    }

    public static int StandardFunction(int[] nums)
    {
        var max1 = int.MinValue;
        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[i] >= max1)
            {
                max1 = nums[i];
            }
        }
        if (nums.Length == 1 || nums.Length == 2)
        {
            return max1;
        }

        var max2 = int.MinValue;
        var findMax2 = false;
        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[i] >= max2 && nums[i] != max1)
            {
                max2 = nums[i];
                findMax2 = true;
            }
        }

        var max3 = int.MinValue;
        var findMax3 = false;
        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[i] >= max3 && nums[i] != max1 && findMax2 && nums[i] != max2)
            {
                max3 = nums[i];
                findMax3 = true;
            }
        }

        if (findMax3)
        {
            return max3;
        }
        return max1;
    }
}

public class ThirdMaxTestCase(int expected, int[] arr, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
