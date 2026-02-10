using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class MoveZeroesTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(nums) => {{{code} MoveZeroes(nums); return nums;}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int[]>>(str, _options);
        var testCases = new List<MoveZeroesTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new MoveZeroesTestCase((int[])pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { new[] {1,3,12,0,0}, new[] {0,1,0,3,12}, true };
        yield return new object[] { new[] {1,2,3,0,0}, new[] {1,2,3,0,0}, false };
        yield return new object[] { Array.Empty<int>(), Array.Empty<int>(), false };
        yield return new object[] { new[] {0}, new[] {0}, false };
        yield return new object[] { new[] {1,0}, new[] {1,0}, false };
        yield return new object[] { new[] {1,0}, new[] {0,1}, false };
        yield return new object[] { new[] {1,0,0}, new[] {0,0,1}, false };
        yield return new object[] { new[] {1,2,3,0,0}, new[] {1,0,2,0,3}, false };
        yield return new object[] { new[] {0,0,0}, new[] {0,0,0}, false };
        yield return new object[] { new[] {1,2,3,4,0,0}, new[] {1,2,0,3,0,4}, false };
        yield return new object[] { new[] {5,6,7,0,0}, new[] {0,5,0,6,7}, false };
        yield return new object[] { new[] {9,0,0,0}, new[] {9,0,0,0}, false };
        yield return new object[] { new[] {1,2,3,0}, new[] {0,1,2,3}, false };
        yield return new object[] { new[] {4,5,6,7,0,0,0}, new[] {4,0,5,0,6,0,7}, false };
        yield return new object[] { new[] {1,1,1}, new[] {1,1,1}, false };
    }

    public static int[] StandardFunction(int[] nums)
    {
        var writeIdx = 0;
        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[i] != 0)
            {
                nums[writeIdx] = nums[i];
                writeIdx++;
            }
        }

        for (int i = nums.Length - 1; i > writeIdx - 1; i--)
        {
            nums[i] = 0;
        }

        return nums;
    }
}

public class MoveZeroesTestCase(int[] expected, int[] arr, bool isPublic)
    : TestCaseBase<int[]>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
