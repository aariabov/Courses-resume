using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class MinTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");
    
    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(a, b) => {{{code} return Min(a, b);}}";
        Func<int, int, int> func = await CSharpScript.EvaluateAsync<Func<int, int, int>>(str, _options);
        
        var testCases = new List<MinTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new MinTestCase((int)pars[0], (int)pars[1], (int)pars[2], (bool)pars[3]));
        }
        return TestsRunner.RunTests((testCase) => func(testCase.Par1, testCase.Par2), testCases);
    }
    
    public static IEnumerable<object[]> TestData()
    {
        yield return [3, 3, 5, true];       // a < b → a
        yield return [7, 7, 7, true];       // a == b → a
        yield return [4, 10, 4, true];      // a > b → b
        yield return [-5, -5, 2, true];     // отрицательное меньше положительного
        yield return [-3, 0, -3, true];     // ноль и отрицательное
        yield return [-20, -10, -20, true]; // оба отрицательные
        yield return [int.MinValue, int.MaxValue, int.MinValue, false]; // крайние значения
        yield return [int.MinValue, int.MinValue, int.MaxValue, false];
        yield return [0, 0, 0, false];       // равные нули
        yield return [0, 1, 0, false];       // положительное и ноль
        yield return [-1, -1, 0, false];     // отрицательное и ноль
        yield return [-100, -100, -50, false]; // сравнение двух отрицательных
    }

    public static int StandardFunction(int a, int b)
    {
        return (a < b) ? a : b;
    }
}

public class MinTestCase(int expected, int par1, int par2, bool isPublic) : TestCaseBase<int>(expected, isPublic)
{
    public int Par1 { get; } = par1;
    public int Par2 { get; } = par2;
    public override string[] Parameters => [Par1.ToString(), Par2.ToString()];
}