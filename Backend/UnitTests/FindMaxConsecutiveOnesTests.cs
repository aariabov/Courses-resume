using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class FindMaxConsecutiveOnesTests
{
    [TestMethod]
    [DynamicData(
        nameof(FindMaxConsecutiveOnesTester.TestData),
        typeof(FindMaxConsecutiveOnesTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int expected, int[] arr, bool _)
    {
        var res = FindMaxConsecutiveOnesTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}
