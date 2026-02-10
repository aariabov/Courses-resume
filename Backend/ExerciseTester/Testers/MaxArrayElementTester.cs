using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class MaxArrayElementTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(a) => {{{code} return MaxArrayElement(a);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int>>(str, _options);
        var testCases = new List<MaxArrayElementTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new MaxArrayElementTestCase((int)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Par1), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { 0, new[] { 0 }, true };
        yield return new object[] { 1, new[] { 1 }, true };
        yield return new object[] { -1, new[] { -1 }, true };
        yield return new object[] { 3, new[] { 1, 2, 3 }, true };
        yield return new object[] { -1, new[] { -3, -2, -1 }, true };
        yield return new object[] { 5, new[] { 5, 5, 5 }, true };
        yield return new object[] { 40, new[] { 10, 20, 30, 40 }, true };
        yield return new object[] { int.MaxValue, new[] { int.MaxValue, 0, -1 }, false };
        yield return new object[] { -1, new[] { int.MinValue, -2, -1 }, false };
        yield return new object[] { 0, new[] { 0, -1, -2 }, false };
    }

    public static int StandardFunction(int[] a)
    {
        var max = a[0];
        for (var i = 1; i < a.Length; i++)
        {
            if (a[i] > max)
            {
                max = a[i];
            }
        }
        return max;
    }
}

public class MaxArrayElementTestCase(int expected, int[] par1, bool isPublic) : TestCaseBase<int>(expected, isPublic)
{
    public int[] Par1 { get; } = par1;
    public override string[] Parameters => [$"[{string.Join(", ", Par1)}]"];
}



