using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class CheckIfExistDoubleTests
{
    [TestMethod]
    [DynamicData(
        nameof(CheckIfExistDoubleTester.TestData),
        typeof(CheckIfExistDoubleTester),
        DynamicDataSourceType.Method
    )]
    public void Test(bool expected, int[] arr, bool _)
    {
        var res = CheckIfExistDoubleTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}
