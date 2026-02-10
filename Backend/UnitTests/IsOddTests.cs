using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class IsOddTests
{
	[TestMethod]
    [DynamicData(nameof(IsOddTester.TestData), typeof(IsOddTester), DynamicDataSourceType.Method)]
    public void Test(bool expected, int number, bool _)
    {
        var res = IsOddTester.StandardFunction(number);
        Assert.AreEqual(expected, res);
    }
}


