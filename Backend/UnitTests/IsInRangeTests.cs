using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class IsInRangeTests
{
    [TestMethod]
    [DynamicData(nameof(IsInRangeTester.TestData), typeof(IsInRangeTester), DynamicDataSourceType.Method)]
    public void Test(bool expected, int val, int min, int max, bool _)
    {
        var res = IsInRangeTester.StandardFunction(val, min, max);
        Assert.AreEqual(expected, res);
    }
}



