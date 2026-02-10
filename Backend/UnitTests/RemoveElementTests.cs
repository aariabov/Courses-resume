using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class RemoveElementTests
{
    [TestMethod]
    [DynamicData(
        nameof(RemoveElementTester.TestData),
        typeof(RemoveElementTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int expected, int[] arr, int val, bool _)
    {
        var res = RemoveElementTester.StandardFunction(arr, val);
        Assert.AreEqual(expected, res);
    }
}
