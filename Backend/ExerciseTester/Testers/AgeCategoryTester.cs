using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class AgeCategoryTester : ITester
{
	private readonly ScriptOptions _options = ScriptOptions.Default
		.AddImports("System")
		.AddReferences("System", "System.Core", "Microsoft.CSharp");

	public async Task<FunctionTestingResult> TestClientCode(string code)
	{
		var str = $"(age) => {{{code} return AgeCategory(age);}}";
		var deleg = await CSharpScript.EvaluateAsync<Func<int, string>>(str, _options);
		var testCases = new List<AgeCategoryTestCase>();
		foreach (var pars in TestData())
		{
			testCases.Add(new AgeCategoryTestCase((string)pars[0], (int)pars[1], (bool)pars[2]));
		}
		return TestsRunner.RunTests((testCase) => deleg(testCase.Age), testCases);
	}

	public static IEnumerable<object[]> TestData()
	{
		// Boundaries and typical values
		yield return new object[] { "ребенок", 0, true };
		yield return new object[] { "ребенок", 5, true };
		yield return new object[] { "подросток", 12, true };
		yield return new object[] { "подросток", 17, true };
		yield return new object[] { "взрослый", 18, true };
		yield return new object[] { "взрослый", 30, true };
		yield return new object[] { "пенсионер", 65, true };
		yield return new object[] { "пенсионер", 80, true };

		// Hidden edge values
		yield return new object[] { "ребенок", 11, false };
		yield return new object[] { "подросток", 16, false };
		yield return new object[] { "взрослый", 64, false };
		yield return new object[] { "пенсионер", 100, false };
	}

	public static string StandardFunction(int age)
	{
		if (age < 12)
			return "ребенок";
		else if (age < 18)
			return "подросток";
		else if (age < 65)
			return "взрослый";
		else
			return "пенсионер";
	}
}

public class AgeCategoryTestCase(string expected, int age, bool isPublic)
	: TestCaseBase<string>(expected, isPublic)
{
	public int Age { get; } = age;
	public override string[] Parameters => [Age.ToString()];
}


