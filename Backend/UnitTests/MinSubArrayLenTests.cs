using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class MinSubArrayLenTests
{
    [TestMethod]
    [DynamicData(
        nameof(MinSubArrayLenTester.TestData),
        typeof(MinSubArrayLenTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int expected, int target, int[] arr, bool _)
    {
        var res = MinSubArrayLenTester.StandardFunction(target, arr);
        Assert.AreEqual(expected, res);
    }
}
