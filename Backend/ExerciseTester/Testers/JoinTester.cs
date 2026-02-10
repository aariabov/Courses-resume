using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class JoinTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System", "System.IO", "System.Collections.Generic",
            "System.Console", "System.Diagnostics", "System.Dynamic",
            "System.Linq", "System.Text",
            "System.Threading.Tasks")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(a) => {{{code} return Join(a);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int[], string>>(str, _options);
        var testCases = new List<JoinTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new JoinTestCase((string)pars[0], (int[])pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Par1), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { "", Array.Empty<int>(), true };
        yield return new object[] { "0", new int[] { 0 }, true };
        yield return new object[] { "1", new int[] { 1 }, true };
        yield return new object[] { "-1", new int[] { -1 }, true };
        yield return new object[] { "1,2,3", new int[] { 1, 2, 3 }, true };
        yield return new object[] { "-1,-2,-3", new int[] { -1, -2, -3 }, true };
        yield return new object[] { "10,20,30,40,50", new int[] { 10, 20, 30, 40, 50 }, true };
        yield return new object[] { "5,0,5", new int[] { 5, 0, 5 }, true };
        yield return new object[] { "1,-1,0,2,-2", new int[] { 1, -1, 0, 2, -2 }, true };
        yield return new object[] { "42,42,42,42,42,42", new int[] { 42, 42, 42, 42, 42, 42 }, true };
        yield return new object[] { "2147483647,-2147483648", new int[] { int.MaxValue, int.MinValue }, true };
        yield return new object[] { "0,1,-1,2,-2,3,-3", new int[] { 0, 1, -1, 2, -2, 3, -3 }, true };
        yield return new object[] { "100,200,300,400,500,600,700,800,900,1000", new int[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 }, true };
        yield return new object[] { "-10,-20,-30,-40,-50", new int[] { -10, -20, -30, -40, -50 }, true };
        yield return new object[] { "0,0,0,0,0", new int[] { 0, 0, 0, 0, 0 }, true };
        yield return new object[] { "1,2,3,4,5,6,7,8,9,10", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, true };
        yield return new object[] { "-1,-2,-3,-4,-5", new int[] { -1, -2, -3, -4, -5 }, true };
        yield return new object[] { "1234567890", new int[] { 1234567890 }, true };
        yield return new object[] { "1,0,0,1", new int[] { 1, 0, 0, 1 }, true };
        yield return new object[] { "-1000,0,1000", new int[] { -1000, 0, 1000 }, true };
        yield return new object[] { "7,14,21,28", new int[] { 7, 14, 21, 28 }, true };
        yield return new object[] { "999,888,777,666,555", new int[] { 999, 888, 777, 666, 555 }, true };
        yield return new object[] { "0,1,2,3,4,5,6,7,8,9,10", new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, true };
        yield return new object[] { "-2147483648,2147483647", new int[] { int.MinValue, int.MaxValue }, true };
        yield return new object[] { "1,1,1,1,1,1,1,1,1,1,1", new int[] { 1,1,1,1,1,1,1,1,1,1,1 }, true };
    }

    public static string StandardFunction(int[] a)
    {
        return string.Join(',', a);
    }
}

public class JoinTestCase(string expected, int[] par1, bool isPublic) : TestCaseBase<string>(expected, isPublic)
{
    public int[] Par1 { get; } = par1;
    public override string[] Parameters => [$"[{string.Join(", ", Par1)}]"];
}
