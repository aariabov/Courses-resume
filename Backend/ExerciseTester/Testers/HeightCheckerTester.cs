using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class HeightCheckerTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(heights) => {{{code} return HeightChecker(heights);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int>>(str, _options);
        var testCases = new List<HeightCheckerTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new HeightCheckerTestCase((int)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Arr), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { 3, new[] {1,1,4,2,1,3}, true };
        yield return new object[] { 0, new[] {1,2,3,4,5}, false };
        yield return new object[] { 0, Array.Empty<int>(), false };
        yield return new object[] { 0, new[] {1}, false };
        yield return new object[] { 2, new[] {2,1}, false };
        yield return new object[] { 0, new[] {1,1,1}, false };
        yield return new object[] { 2, new[] {1,3,2}, false };
        yield return new object[] { 4, new[] {4,3,2,1}, false };
        yield return new object[] { 2, new[] {1,2,2,1}, false };
        yield return new object[] { 5, new[] {5,1,2,3,4}, false };
        yield return new object[] { 3, new[] {1,2,3,2,1}, false };
        yield return new object[] { 2, new[] {100,90,90,100}, false };
        yield return new object[] { 4, new[] {2,1,3,1,2}, false };
        yield return new object[] { 2, new[] {1,2,4,3,5}, false };
        yield return new object[] { 3, new[] {9,7,7,8}, false };
    }

    public static int StandardFunction(int[] heights)
    {
        var result = new int[heights.Length];
        Array.Copy(heights, result, heights.Length);
        Array.Sort(heights);

        var k = 0;
        for (int i = 0; i < heights.Length; i++)
        {
            if (heights[i] != result[i])
            {
                k++;
            }
        }
        return k;
    }
}

public class HeightCheckerTestCase(int expected, int[] arr, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int[] Arr { get; } = arr;
    public override string[] Parameters => [$"[{string.Join(", ", Arr)}]"];
}
