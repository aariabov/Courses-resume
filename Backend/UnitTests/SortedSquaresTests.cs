using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class SortedSquaresTests
{
    [TestMethod]
    [DynamicData(
        nameof(SortedSquaresTester.TestData),
        typeof(SortedSquaresTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int[] expected, int[] arr, bool _)
    {
        var res = SortedSquaresTester.StandardFunction(arr);
        CollectionAssert.AreEqual(expected, res);
    }
}
