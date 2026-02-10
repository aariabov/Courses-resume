using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class IsInRangeTester : ITester
{
    private readonly ScriptOptions _options = ScriptOptions.Default
        .AddImports("System")
        .AddReferences("System", "System.Core", "Microsoft.CSharp");

    public async Task<FunctionTestingResult> TestClientCode(string code)
    {
        var str = $"(val, min, max) => {{{code} return IsInRange(val, min, max);}}";
        var deleg = await CSharpScript.EvaluateAsync<Func<int, int, int, bool>>(str, _options);
        var testCases = new List<IsInRangeTestCase>();
        foreach (var pars in TestData())
        {
            testCases.Add(new IsInRangeTestCase((bool)pars[0], (int)pars[1], (int)pars[2], (int)pars[3], (bool)pars[4]));
        }
        return TestsRunner.RunTests((testCase) => deleg(testCase.Val, testCase.Min, testCase.Max), testCases);
    }

    public static IEnumerable<object[]> TestData()
    {
        // Public tests
        yield return new object[] { true, 5, 1, 10, true };   // inside
        yield return new object[] { true, 1, 1, 10, true };    // boundary min
        yield return new object[] { true, 10, 1, 10, true };   // boundary max
        yield return new object[] { false, 0, 1, 10, true };   // below

        // Hidden tests
        yield return new object[] { false, 11, 1, 10, false }; // above
        yield return new object[] { true, -5, -10, -1, false }; // negatives
        yield return new object[] { true, 0, 0, 0, false }; // single point
        yield return new object[] { true, int.MinValue, int.MinValue, int.MinValue, false };
        yield return new object[] { true, int.MaxValue, int.MinValue, int.MaxValue, false };
    }

    public static bool StandardFunction(int val, int min, int max)
    {
        return val >= min && val <= max;
    }
}

public class IsInRangeTestCase(bool expected, int val, int min, int max, bool isPublic)
    : TestCaseBase<bool>(expected, isPublic)
{
    public int Val { get; } = val;
    public int Min { get; } = min;
    public int Max { get; } = max;
    public override string[] Parameters => [Val.ToString(), Min.ToString(), Max.ToString()];
}


