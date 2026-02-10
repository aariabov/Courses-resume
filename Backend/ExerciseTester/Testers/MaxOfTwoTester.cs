using Common;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ExerciseTester.Testers;

public class MaxOfTwoTester : ITester
{
	private readonly ScriptOptions _options = ScriptOptions.Default
		.AddImports("System")
		.AddReferences("System", "System.Core", "Microsoft.CSharp");

	public async Task<FunctionTestingResult> TestClientCode(string code)
	{
		var str = $"(a, b) => {{{code} return MaxOfTwo(a, b);}}";
		Func<int, int, int> func = await CSharpScript.EvaluateAsync<Func<int, int, int>>(str, _options);

		var testCases = new List<MaxOfTwoTestCase>();
		foreach (var pars in TestData())
		{
			testCases.Add(new MaxOfTwoTestCase((int)pars[0], (int)pars[1], (int)pars[2], (bool)pars[3]));
		}
		return TestsRunner.RunTests((testCase) => func(testCase.A, testCase.B), testCases);
	}

	public static IEnumerable<object[]> TestData()
	{
		yield return new object[] { 5, 5, 3, true };    // a > b
		yield return new object[] { 7, 7, 7, true };    // a == b
		yield return new object[] { 10, 4, 10, true };  // a < b
		yield return new object[] { 0, 0, -1, true };   // zero vs negative

		yield return new object[] { 2, 2, -2, false };
		yield return new object[] { 100, -100, 100, false };
		yield return new object[] { int.MaxValue, int.MaxValue, int.MinValue, false };
		yield return new object[] { 0, -1, 0, false };
	}

	public static int StandardFunction(int a, int b)
	{
		if (a > b)
			return a;
		else
			return b;
	}
}

public class MaxOfTwoTestCase(int expected, int a, int b, bool isPublic) : TestCaseBase<int>(expected, isPublic)
{
	public int A { get; } = a;
	public int B { get; } = b;
	public override string[] Parameters => [A.ToString(), B.ToString()];
}


