using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class ReplaceElementsTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(arr) => {{{code} return ReplaceElements(arr);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int[]>>(str, _options);
        var testCases = new List<ReplaceElementsTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new ReplaceElementsTestCase((int[])pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { new[] {3,1,-1,-1}, new[] {2,3,1,-1}, true };
        yield return new object[] { new[] {-1}, new[] {5}, false };
        yield return new object[] { new[] {18,6,6,6,1,-1}, new[] {17,18,5,4,6,1}, false };
        yield return new object[] { new[] {3,3,-1}, new[] {1,2,3}, false };
        yield return new object[] { Array.Empty<int>(), Array.Empty<int>(), false };
        yield return new object[] { new[] {-1}, new[] {0}, false };
        yield return new object[] { new[] {-1,-1,-1}, new[] {-1,-2,-3}, false };
        yield return new object[] { new[] {4,3,2,1,-1}, new[] {5,4,3,2,1}, false };
        yield return new object[] { new[] {100,50,50,-1}, new[] {1,100,2,50}, false };
        yield return new object[] { new[] {2,2,-1}, new[] {2,2,2}, false };
        yield return new object[] { new[] {9,9,-1}, new[] {9,1,9}, false };
        yield return new object[] { new[] {17,0,-1}, new[] {0,17,0}, false };
        yield return new object[] { new[] {500,250,125,-1}, new[] {1000,500,250,125}, false };
        yield return new object[] { new[] {4,4,4,4,-1}, new[] {1,1,2,3,4}, false };
        yield return new object[] { new[] {2147483647,2147483647,-1}, new[] {int.MinValue, 0, int.MaxValue}, false };
    }

    public static int[] StandardFunction(int[] arr)
    {
        int maxRight = -1;
        for (int i = arr.Length - 1; i >= 0; i--)
        {
            int temp = arr[i];
            arr[i] = maxRight;
            maxRight = Math.Max(maxRight, temp);
        }

        return arr;
    }
}

public class ReplaceElementsTestCase(int[] expected, int[] arr, bool isPublic)
    : TestCaseBase<int[]>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
