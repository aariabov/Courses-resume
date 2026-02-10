using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class SquareTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports(
            "System",
            "System.IO",
            "System.Collections.Generic",
            "System.Console",
            "System.Diagnostics",
            "System.Dynamic",
            "System.Linq",
            "System.Text",
            "System.Threading.Tasks"
        )
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(a) => {{{code} return Square(a);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int, int>>(str, _options);
        var testCases = new List<SquareTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new SquareTestCase((int)pars[0], (int)pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Par1), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { 0, 0, true };
        yield return new object[] { 1, 1, true };
        yield return new object[] { 4, 2, true };
        yield return new object[] { 9, 3, true };
        yield return new object[] { 16, 4, true };
        yield return new object[] { 25, 5, true };
        yield return new object[] { 100, 10, true };
        yield return new object[] { 144, 12, true };
        yield return new object[] { 10000, 100, true };
        yield return new object[] { 2147395600, 46340, true }; // квадрат максимально большого int без переполнения
        yield return new object[] { 1, -1, true };
        yield return new object[] { 4, -2, true };
        yield return new object[] { 9, -3, true };
        yield return new object[] { 16, -4, true };
        yield return new object[] { 2147395600, -46340, true }; // аналогично положительному максимуму
    }

    public static int StandardFunction(int a)
    {
        return a * a;
    }
}

public class SquareTestCase(int expected, int par1, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int Par1 { get; } = par1;
    public override string[] Parameters => [Par1.ToString()];
}
