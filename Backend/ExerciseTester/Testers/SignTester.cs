using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class SignTester : ITester
{
	private readonly ScriptOptions _options = ScriptOptions.Default
		.AddImports("System")
		.AddReferences("System", "System.Core", "Microsoft.CSharp");

	public async Task<FunctionTestingResult> TestClientCode(string code)
	{
		var str = $"(number) => {{{code} return Sign(number);}}";
		var deleg = await CSharpScript.EvaluateAsync<Func<int, string>>(str, _options);
		var testCases = new List<SignTestCase>();
		foreach (var pars in TestData())
		{
			testCases.Add(new SignTestCase((string)pars[0], (int)pars[1], (bool)pars[2]));
		}
		return TestsRunner.RunTests((testCase) => deleg(testCase.Number), testCases);
	}

	public static IEnumerable<object[]> TestData()
	{
		yield return new object[] { "положительное", 1, true };
		yield return new object[] { "отрицательное", -1, true };
		yield return new object[] { "ноль", 0, true };

		yield return new object[] { "положительное", 100, false };
		yield return new object[] { "отрицательное", -100, false };
		yield return new object[] { "положительное", int.MaxValue, false };
		yield return new object[] { "отрицательное", int.MinValue, false };
	}

	public static string StandardFunction(int number)
	{
		if (number > 0)
			return "положительное";
		else if (number < 0)
			return "отрицательное";
		else
			return "ноль";
	}
}

public class SignTestCase(string expected, int number, bool isPublic)
	: TestCaseBase<string>(expected, isPublic)
{
	public int Number { get; } = number;
	public override string[] Parameters => [Number.ToString()];
}


