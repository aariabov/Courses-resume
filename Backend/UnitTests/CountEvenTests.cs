using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class CountEvenTests
{
    [TestMethod]
    [DynamicData(nameof(CountEvenTester.TestData), typeof(CountEvenTester), DynamicDataSourceType.Method)]
    public void Test(int expected, int[] arr, bool _)
    {
        var res = CountEvenTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}





