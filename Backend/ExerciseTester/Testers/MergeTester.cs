using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class MergeTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(nums1, m, nums2, n) => {{{code} Merge(nums1, m, nums2, n); return nums1;}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int, int[], int, int[]>>(str, _options);
        var testCases = new List<MergeTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new MergeTestCase((int[])pars[0], (int[])pars[1], (int)pars[2], (int[])pars[3], (int)pars[4]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Nums1, testCase.M, testCase.Nums2, testCase.N), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { new[] {1,2,2,3,0,0}, new[] {1,2,0,0,0,0}, 2, new[] {2,3}, 2 };
        yield return new object[] { new[] {1,3,4,5,6}, new[] {1,0,0,0,0}, 1, new[] {3,4,5,6}, 4 };
        yield return new object[] { new[] {1}, new[] {0}, 0, new[] {1}, 1 };
        yield return new object[] { new[] {1,2}, new[] {1,0}, 1, new[] {2}, 1 };
        yield return new object[] { new[] {1,2}, new[] {2,0}, 1, new[] {1}, 1 };
        yield return new object[] { new[] {1,2,3}, new[] {0,0,0}, 0, new[] {1,2,3}, 3 };
        yield return new object[] { new[] {1,2,3,4,5,6}, new[] {1,2,3,0,0,0}, 3, new[] {4,5,6}, 3 };
        yield return new object[] { new[] {1,2,3,4,5,6}, new[] {4,5,6,0,0,0}, 3, new[] {1,2,3}, 3 };
        yield return new object[] { new[] {1,2,3,4,5}, new[] {1,3,5,0,0}, 3, new[] {2,4}, 2 };
        yield return new object[] { new[] {-2,-1,0}, new[] {-1,0,0}, 1, new[] {-2,0}, 2 };
        yield return new object[] { new[] {-3,-2,-1,0}, new[] {-3,-1,0,0}, 3, new[] {-2}, 1 };
        yield return new object[] { new[] {1,2,3,4}, new[] {1,2,4,0}, 3, new[] {3}, 1 };
        yield return new object[] { new[] {1,1,1,1,1}, new[] {1,1,1,0,0}, 3, new[] {1,1}, 2 };
        yield return new object[] { new[] {1,2,3,4,7}, new[] {1,4,7,0,0}, 3, new[] {2,3}, 2 };
        yield return new object[] { new[] {1,2,3,4,5,6}, new[] {2,3,5,0,0,0}, 3, new[] {1,4,6}, 3 };
    }

    public static int[] StandardFunction(int[] nums1, int m, int[] nums2, int n)
    {
        var res = new int[m + n];
        Array.Copy(nums1, 0, res, 0, m);
        int i = 0, j = 0, k = 0;
        while (i < m && j < n)
        {
            if (res[i] <= nums2[j])
            {
                nums1[k++] = res[i++];
            }
            else
            {
                nums1[k++] = nums2[j++];
            }
        }
        while (i < m) nums1[k++] = res[i++];
        while (j < n) nums1[k++] = nums2[j++];
        return nums1;
    }
}

public class MergeTestCase(int[] expected, int[] nums1, int m, int[] nums2, int n)
    : TestCaseBase<int[]>(expected, true)
{
    public int[] Nums1 { get; } = nums1;
    public int M { get; } = m;
    public int[] Nums2 { get; } = nums2;
    public int N { get; } = n;
    public override string[] Parameters => new[] { $"nums1=[{string.Join(", ", Nums1)}]", $"m={M}", $"nums2=[{string.Join(", ", Nums2)}]", $"n={N}" };
}
