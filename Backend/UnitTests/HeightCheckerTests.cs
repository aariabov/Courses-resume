using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class HeightCheckerTests
{
    [TestMethod]
    [DynamicData(
        nameof(HeightCheckerTester.TestData),
        typeof(HeightCheckerTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int expected, int[] arr, bool _)
    {
        var res = HeightCheckerTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}
