using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class AddTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System", "System.IO", "System.Collections.Generic",
            "System.Console", "System.Diagnostics", "System.Dynamic",
            "System.Linq", "System.Text",
            "System.Threading.Tasks")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");
    
    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(a, b) => {{{code} return Add(a, b);}}";
        Func<int, int, int> func = await CSharpScript.EvaluateAsync<Func<int, int, int>>(str, _options);
        
        var testCases = new List<AddTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new AddTestCase((int)pars[0], (int)pars[1], (int)pars[2], (bool)pars[3]));
        }
        return TestsRunner.RunTests((testCase) => func(testCase.Par1, testCase.Par2), testCases);
    }
    
    public static IEnumerable<object[]> TestData()
    {
        yield return [3, 2, 1, true];
        yield return [5, 2, 3, false];
        yield return [6, 2, 4, true];
        yield return [3, 3, 0, true];
        yield return [0, 0, 0, true];
        yield return [1, 1, 0, true];
        yield return [1, 0, 1, true];
        yield return [10, 7, 3, true];
        yield return [15, 8, 7, true];
        yield return [100, 50, 50, true];
        yield return [-5, -2, -3, true];
        yield return [-10, -7, -3, true];
        yield return [0, 5, -5, true];
        yield return [2, 5, -3, true];
        yield return [-2, -5, 3, true];
        yield return [1000, 600, 400, true];
        yield return [-100, -50, -50, true];
        yield return [int.MaxValue, int.MaxValue, 0, true];
        yield return [int.MinValue + 1, int.MinValue, 1, true];
        yield return [-1, int.MaxValue, int.MinValue, true];
        yield return [0, int.MaxValue, int.MinValue + 1, true];
    }

    public static int StandardFunction(int a, int b)
    {
        return a + b;
    }
}

public class AddTestCase(int expected, int par1, int par2, bool isPublic) : TestCaseBase<int>(expected, isPublic)
{
    public int Par1 { get; } = par1;
    public int Par2 { get; } = par2;
    public override string[] Parameters => [Par1.ToString(), Par2.ToString()];
}