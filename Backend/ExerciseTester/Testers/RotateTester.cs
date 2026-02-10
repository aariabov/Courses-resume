using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class RotateTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(nums, k) => {{{code} Rotate(nums, k);return nums;}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int, int[]>>(str, _options);
        var testCases = new List<RotateTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(
                new RotateTestCase((int[])pars[0], (int[])pars[1], (int)pars[2], (bool)pars[3])
            );
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr, testCase.K), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        // public
        yield return new object[] { new[] { 3, 4, 5, 1, 2 }, new[] { 1, 2, 3, 4, 5 }, 3, true };
        yield return new object[] { new[] { 5, 1, 2, 3, 4 }, new[] { 1, 2, 3, 4, 5 }, 1, true };
        yield return new object[] { new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5 }, 5, true };
        yield return new object[] { new int[] { }, new int[] { }, 0, true };
        yield return new object[] { new[] { 3, 4, 5, 1, 2 }, new[] { 1, 2, 3, 4, 5 }, 3, true };
        yield return new object[] { new[] { 1 }, new[] { 1 }, 0, true };
        yield return new object[] { new[] { 5, 1, 2, 3, 4 }, new[] { 1, 2, 3, 4, 5 }, 1, true };
        yield return new object[] { new[] { 1, 2, 3, 4 }, new[] { 1, 2, 3, 4 }, 4, true };
        yield return new object[] { new[] { 4, 5, 1, 2, 3 }, new[] { 1, 2, 3, 4, 5 }, 7, true };
        yield return new object[] { new[] { 2, 2, 1, 1 }, new[] { 1, 1, 2, 2 }, 2, true };
        yield return new object[] { new[] { 2, 1 }, new[] { 1, 2 }, 1, true };
        yield return new object[] { new[] { 3, 4, 1, 2 }, new[] { 1, 2, 3, 4 }, 2, true };
        yield return new object[] { new[] { 9, 1, 2, 3, 4 }, new[] { 1, 2, 3, 4, 9 }, 1, true };
        yield return new object[] { new[] { 10, 20, 30 }, new[] { 10, 20, 30 }, 3000, true };

        // hidden
        yield return new object[] { new[] { 3, 4, 1, 2 }, new[] { 1, 2, 3, 4 }, 2, false };
        yield return new object[] { new[] { -2, -3, -1 }, new[] { -1, -2, -3 }, 2, false };
        yield return new object[] { new[] { 7, 7, 7, 7 }, new[] { 7, 7, 7, 7 }, 2, false };
        yield return new object[] { new[] { 5, 6, 7 }, new[] { 5, 6, 7 }, 0, false };
        yield return new object[] { new[] { 8, 7 }, new[] { 7, 8 }, 3, false };
        yield return new object[] { new[] { 4, 1, 2, 3 }, new[] { 1, 2, 3, 4 }, 1, false };
        yield return new object[]
        {
            new[] { 9, 1, 2, 3, 4, 5, 6, 7, 8 },
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            1,
            false
        };
        yield return new object[]
        {
            new[] { 6, 7, 8, 9, 1, 2, 3, 4, 5 },
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            4,
            false
        };
        yield return new object[] { new[] { 4, 5, 1, 2, 3 }, new[] { 1, 2, 3, 4, 5 }, 2, false };
        yield return new object[]
        {
            new[] { 2, 3, 4, 5, 6, 1 },
            new[] { 1, 2, 3, 4, 5, 6 },
            1000001,
            false
        };
        yield return new object[] { new[] { 42 }, new[] { 42 }, 1000, false };
    }

    public static void StandardFunction(int[] nums, int k)
    {
        if (k == 0 || nums.Length == 1 || k == nums.Length)
        {
            return;
        }

        if (k > nums.Length)
        {
            k = k % nums.Length;
        }

        Reverse(0, nums.Length - 1);
        Reverse(0, k - 1);
        Reverse(k, nums.Length - 1);

        void Reverse(int left, int right)
        {
            while (left < right)
            {
                (nums[left], nums[right]) = (nums[right], nums[left]);
                left++;
                right--;
            }
        }
    }
}

public class RotateTestCase(int[] expected, int[] arr, int k, bool isPublic)
    : TestCaseBase<int[]>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public int K { get; } = k;
    public override string[] Parameters => new[] { $"[{string.Join(", ", Arr)}]", K.ToString() };
}
