using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class DuplicateZerosTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(arr) => {{{code} DuplicateZeros(arr); return arr;}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int[]>>(str, _options);
        var testCases = new List<DuplicateZerosTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new DuplicateZerosTestCase((int[])pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { new[] {1,0,0,2,3}, new[] {1,0,2,3,0}, true };
        yield return new object[] { new[] {1,0,0,0,0}, new[] {1,0,0,0,0}, false };
        yield return new object[] { Array.Empty<int>(), Array.Empty<int>(), false };
        yield return new object[] { new[] {0}, new[] {0}, false };
        yield return new object[] { new[] {0,0}, new[] {0,1}, false };
        yield return new object[] { new[] {1,0}, new[] {1,0}, false };
        yield return new object[] { new[] {8,4,5,0,0,0}, new[] {8,4,5,0,0,2}, false };
        yield return new object[] { new[] {0,0,0,0}, new[] {0,0,0,0}, false };
        yield return new object[] { new[] {1,2,3,4,5}, new[] {1,2,3,4,5}, false };
        yield return new object[] { new[] {0,0,1,7,0,0}, new[] {0,1,7,0,2,3}, false };
        yield return new object[] { new[] {9,0,0,0,0}, new[] {9,0,0,3,0}, false };
        yield return new object[] { new[] {0,0,1}, new[] {0,1,0}, false };
        yield return new object[] { new[] {1,0,0}, new[] {1,0,0}, false };
        yield return new object[] { new[] {2,3,0,0}, new[] {2,3,0,4}, false };
        yield return new object[] { new[] {7,0,0,8,0}, new[] {7,0,8,0,9}, false };
    }

    public static int[] StandardFunction(int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == 0)
            {
                for (int j = arr.Length - 1; j > i; j--)
                {
                    arr[j] = arr[j - 1];
                }

                if (i < arr.Length - 1)
                {
                    arr[i + 1] = 0;
                    i = i + 1;
                }
            }
        }

        return arr;
    }
}

public class DuplicateZerosTestCase(int[] expected, int[] arr, bool isPublic)
    : TestCaseBase<int[]>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
