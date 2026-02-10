using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class PivotIndexTests
{
    [TestMethod]
    [DynamicData(
        nameof(PivotIndexTester.TestData),
        typeof(PivotIndexTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int expected, int[] arr, bool _)
    {
        var res = PivotIndexTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}
