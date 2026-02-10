using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class IndexOfTests
{
    [TestMethod]
    [DynamicData(nameof(IndexOfTester.TestData), typeof(IndexOfTester), DynamicDataSourceType.Method)]
    public void Test(int expected, int[] arr, int target, bool _)
    {
        var res = IndexOfTester.StandardFunction(arr, target);
        Assert.AreEqual(expected, res);
    }
}





