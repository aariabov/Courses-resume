using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class DuplicateZerosTests
{
    [TestMethod]
    [DynamicData(
        nameof(DuplicateZerosTester.TestData),
        typeof(DuplicateZerosTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int[] expected, int[] arr, bool _)
    {
        var res = DuplicateZerosTester.StandardFunction(arr);
        CollectionAssert.AreEqual(expected, res);
    }
}
