using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class IsPositiveTests
{
    [TestMethod]
    [DynamicData(nameof(IsPositiveTester.TestData), typeof(IsPositiveTester), DynamicDataSourceType.Method)]
    public void Test(bool expected, int number, bool _)
    {
        var res = IsPositiveTester.StandardFunction(number);
        Assert.AreEqual(expected, res);
    }
}


