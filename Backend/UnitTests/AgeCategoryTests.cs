using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class AgeCategoryTests
{
	[TestMethod]
	[DynamicData(nameof(AgeCategoryTester.TestData), typeof(AgeCategoryTester), DynamicDataSourceType.Method)]
	public void Test(string expected, int age, bool _)
	{
		var res = AgeCategoryTester.StandardFunction(age);
		Assert.AreEqual(expected, res);
	}
}


