using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class PowerTests
{
    [TestMethod]
    [DynamicData(nameof(PowerTester.TestData), typeof(PowerTester), DynamicDataSourceType.Method)]
    public void Test(int expected, int number, int exponent, bool _)
    {
        var res = PowerTester.StandardFunction(number, exponent);
        Assert.AreEqual(expected, res);
    }
}





