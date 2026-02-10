using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class SortedSquaresTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(nums) => {{{code} return SortedSquares(nums);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int[]>>(str, _options);
        var testCases = new List<SortedSquaresTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new SortedSquaresTestCase((int[])pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { new[] { 0, 1, 9 }, new[] { -3, -1, 0 }, true };
        yield return new object[] { new[] { 0 }, new[] { 0 }, true };
        yield return new object[] { new[] { 1, 4, 9 }, new[] { -1, 2, 3 }, false };
        yield return new object[] { new[] { 1, 4, 9, 16, 25 }, new[] { -5, -4, -3, -2, -1 }, false };
        yield return new object[] { new[] { 0, 1, 1, 4, 4 }, new[] { -2, -1, 0, 1, 2 }, false };
        yield return new object[] { new[] { 1, 4, 9, 16 }, new[] { 1, 2, 3, 4 }, false };
        yield return new object[] { new[] { 0, 0, 1, 1 }, new[] { -1, 0, 0, 1 }, false };
        yield return new object[] { new[] { 0, 2500, 2500, 10000, 10000 }, new[] { -100, -50, 0, 50, 100 }, false };
        yield return new object[] { new[] { 1, 4, 4 }, new[] { -2, -2, -1 }, false };
        yield return new object[] { new[] { 4, 9, 100 }, new[] { 2, 3, 10 }, false };
        yield return new object[] { new[] { 4, 9, 100 }, new[] { -10, -3, 2 }, false };
        yield return new object[] { Array.Empty<int>(), Array.Empty<int>(), false };
        yield return new object[] { new[] { 1, 1, 1 }, new[] { -1, -1, -1 }, false };
        yield return new object[] { new[] { 0, 1 }, new[] { 0, 1 }, false };
        yield return new object[] { new[] { 4, 4, 9, 16 }, new[] { -4, -2, 2, 3 }, false };
    }

    public static int[] StandardFunction(int[] nums)
    {
        int n = nums.Length;
        int[] result = new int[n];
        int left = 0, right = n - 1, pos = n - 1;
        while (left <= right)
        {
            int leftVal = nums[left] * nums[left];
            int rightVal = nums[right] * nums[right];
            if (leftVal > rightVal)
            {
                result[pos] = leftVal;
                left++;
            }
            else
            {
                result[pos] = rightVal;
                right--;
            }
            pos--;
        }
        return result;
    }
}

public class SortedSquaresTestCase(int[] expected, int[] arr, bool isPublic)
    : TestCaseBase<int[]>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
