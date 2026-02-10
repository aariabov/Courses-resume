using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class IsPositiveTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(number) => {{{code} return IsPositive(number);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int, bool>>(str, _options);
        var testCases = new List<IsPositiveTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new IsPositiveTestCase((bool)pars[0], (int)pars[1], (bool)pars[2]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Number), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { false, 0, true };
        yield return new object[] { true, 1, true };
        yield return new object[] { true, 10, true };
        yield return new object[] { false, -1, true };

        yield return new object[] { false, -100, false };
        yield return new object[] { true, 100, false };
        yield return new object[] { false, int.MinValue, false };
        yield return new object[] { true, int.MaxValue, false };
    }

    public static bool StandardFunction(int number)
    {
        return number > 0;
    }
}

public class IsPositiveTestCase(bool expected, int number, bool isPublic)
    : TestCaseBase<bool>(expected, isPublic)
{
    public int Number { get; } = number;
    public override string[] Parameters => [Number.ToString()];
}


