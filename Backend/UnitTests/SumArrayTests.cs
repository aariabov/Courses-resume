using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class SumArrayTests
{
    [TestMethod]
    [DynamicData(nameof(SumArrayTester.TestData), typeof(SumArrayTester), DynamicDataSourceType.Method)]
    public void Test(int expected, int[] arr, bool _)
    {
        var res = SumArrayTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}



