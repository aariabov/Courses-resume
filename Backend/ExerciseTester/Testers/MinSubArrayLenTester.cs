using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class MinSubArrayLenTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(target, nums) => {{{code} return MinSubArrayLen(target, nums);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int, int[], int>>(str, _options);
        var testCases = new List<MinSubArrayLenTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(
                new MinSubArrayLenTestCase(
                    (int)pars[0],
                    (int)pars[1],
                    (int[])pars[2],
                    (bool)pars[3]
                )
            );
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Target, testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        // public
        yield return new object[] { 2, 7, new[] { 2, 3, 1, 2, 4, 3 }, true };
        yield return new object[] { 1, 4, new[] { 1, 4, 4 }, true };
        yield return new object[] { 1, 11, new[] { 11 }, true };

        // hidden
        yield return new object[] { 2, 6, new[] { 3, 1, 3, 2, 4, 1 }, false };
        yield return new object[] { 4, 7, new[] { 2, 1, 3, 1, 1 }, false };
        yield return new object[] { 0, 10, new[] { 1, 1, 1, 1 }, false };
    }

    public static int StandardFunction(int target, int[] nums)
    {
        int left = 0;
        int sum = 0;
        int minLen = int.MaxValue;

        for (int right = 0; right < nums.Length; right++)
        {
            // добавляем новый элемент к сумме подмассива слева
            sum += nums[right];

            while (sum >= target)
            {
                var len = right - left + 1; // длина подмассива слева
                if (len < minLen)
                {
                    minLen = len;
                }

                // пробуем уменьшить длину убрав левый элемент (уменьшив окно)
                sum -= nums[left];
                left++;
            }
        }

        return minLen == int.MaxValue ? 0 : minLen;
    }
}

public class MinSubArrayLenTestCase(int expected, int target, int[] arr, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int Target { get; } = target;
    public int[] Arr { get; } = arr;
    public override string[] Parameters =>
        new[] { Target.ToString(), $"[{string.Join(", ", Arr)}]" };
}
