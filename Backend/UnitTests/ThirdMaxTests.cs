using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class ThirdMaxTests
{
    [TestMethod]
    [DynamicData(
        nameof(ThirdMaxTester.TestData),
        typeof(ThirdMaxTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int expected, int[] arr, bool _)
    {
        var res = ThirdMaxTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}
