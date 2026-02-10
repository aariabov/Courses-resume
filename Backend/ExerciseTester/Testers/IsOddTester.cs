using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class IsOddTester : ITester
{
	private readonly ScriptOptions _options = ScriptOptions.Default
		.AddImports("System")
		.AddReferences("System", "System.Core", "Microsoft.CSharp");

	public async Task<FunctionTestingResult> TestClientCode(string code)
	{
		var str = $"(number) => {{{code} return IsOdd(number);}}";
		var deleg = await CSharpScript.EvaluateAsync<Func<int, bool>>(str, _options);
		var testCases = new List<IsOddTestCase>();
		foreach (var pars in TestData())
		{
			testCases.Add(new IsOddTestCase((bool)pars[0], (int)pars[1], (bool)pars[2]));
		}
		return TestsRunner.RunTests((testCase) => deleg(testCase.Number), testCases);
	}

	public static IEnumerable<object[]> TestData()
	{
		// Public cases
		yield return new object[] { false, 0, true };
		yield return new object[] { true, 1, true };
		yield return new object[] { false, 2, true };
		yield return new object[] { true, -1, true };

		// Hidden cases
		yield return new object[] { false, -2, false };
		yield return new object[] { false, 100, false };
		yield return new object[] { true, 101, false };
		yield return new object[] { true, int.MinValue + 1, false };
		yield return new object[] { false, int.MinValue, false };
		yield return new object[] { true, int.MaxValue, false };
	}

	public static bool StandardFunction(int number)
	{
		if (number % 2 == 0)
			return false;
		else
			return true;
	}
}

public class IsOddTestCase(bool expected, int number, bool isPublic)
	: TestCaseBase<bool>(expected, isPublic)
{
	public int Number { get; } = number;
	public override string[] Parameters => [Number.ToString()];
}


