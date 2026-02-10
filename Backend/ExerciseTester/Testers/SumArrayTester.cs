using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class SumArrayTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(a) => {{{code} return SumArray(a);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], int>>(str, _options);
        var testCases = new List<SumArrayTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new SumArrayTestCase((int)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Par1), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { 0, Array.Empty<int>(), true };
        yield return new object[] { 0, new[] { 0 }, true };
        yield return new object[] { 1, new[] { 1 }, true };
        yield return new object[] { -1, new[] { -1 }, true };
        yield return new object[] { 6, new[] { 1, 2, 3 }, true };
        yield return new object[] { -6, new[] { -1, -2, -3 }, true };
        yield return new object[] { 0, new[] { -1, 1 }, true };
        yield return new object[] { 15, new[] { 5, 5, 5 }, true };
        yield return new object[] { 100, new[] { 10, 20, 30, 40 }, true };
        yield return new object[] { int.MaxValue, new[] { int.MaxValue }, false };
        yield return new object[] { int.MinValue, new[] { int.MinValue }, false };
        yield return new object[] { -2, new[] { int.MaxValue, int.MinValue, -1 }, false };
        yield return new object[] { 0, new[] { 0, 0, 0, 0 }, false };
    }

    public static int StandardFunction(int[] a)
    {
        var sum = 0;
        for (var i = 0; i < a.Length; i++)
        {
            sum += a[i];
        }
        return sum;
    }
}

public class SumArrayTestCase(int expected, int[] par1, bool isPublic) : TestCaseBase<int>(expected, isPublic)
{
    public int[] Par1 { get; } = par1;
    public override string[] Parameters => [$"[{string.Join(", ", Par1)}]"];
}



