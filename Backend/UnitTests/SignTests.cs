using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class SignTests
{
	[TestMethod]
	[DynamicData(nameof(SignTester.TestData), typeof(SignTester), DynamicDataSourceType.Method)]
	public void Test(string expected, int number, bool _)
	{
		var res = SignTester.StandardFunction(number);
		Assert.AreEqual(expected, res);
	}
}


