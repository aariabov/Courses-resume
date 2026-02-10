using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class RemoveDuplicatesTests
{
    [TestMethod]
    [DynamicData(
        nameof(RemoveDuplicatesTester.TestData),
        typeof(RemoveDuplicatesTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int expected, int[] arr, bool _)
    {
        var res = RemoveDuplicatesTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}
