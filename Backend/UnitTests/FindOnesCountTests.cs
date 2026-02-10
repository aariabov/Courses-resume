using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class FindOnesCountTests
{
    [TestMethod]
    [DynamicData(
        nameof(FindOnesCountTester.TestData),
        typeof(FindOnesCountTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int expected, int[] arr, bool _)
    {
        var res = FindOnesCountTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}
