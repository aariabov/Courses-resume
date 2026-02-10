using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class MaxOfTwoTests
{
	[TestMethod]
	[DynamicData(nameof(MaxOfTwoTester.TestData), typeof(MaxOfTwoTester), DynamicDataSourceType.Method)]
	public void Test(int expected, int a, int b, bool _)
	{
		var res = MaxOfTwoTester.StandardFunction(a, b);
		Assert.AreEqual(expected, res);
	}
}


