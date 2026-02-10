using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class PowerTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(number, exponent) => {{{code} return Power(number, exponent);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int, int, int>>(str, _options);
        var testCases = new List<PowerTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new PowerTestCase((int)pars[0], (int)pars[1], (int)pars[2], (bool)pars[3]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Number, testCase.Exponent), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        // public
        yield return new object[] { 1, 5, 0, true };
        yield return new object[] { 5, 5, 1, true };
        yield return new object[] { 8, 2, 3, true };
        yield return new object[] { 27, 3, 3, true };

        // hidden
        yield return new object[] { 0, 0, 5, false };
        yield return new object[] { 1, 1, 100, false };
        yield return new object[] { 1024, 2, 10, false };
        yield return new object[] { 59049, 3, 10, false };
    }

    public static int StandardFunction(int number, int exponent)
    {
        var result = 1;
        for (var i = 0; i < exponent; i++)
        {
            result *= number;
        }
        return result;
    }
}

public class PowerTestCase(int expected, int number, int exponent, bool isPublic)
    : TestCaseBase<int>(expected, isPublic)
{
    public int Number { get; } = number;
    public int Exponent { get; } = exponent;
    public override string[] Parameters => [Number.ToString(), Exponent.ToString()];
}





