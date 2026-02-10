using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class MoveZeroesTests
{
    [TestMethod]
    [DynamicData(
        nameof(MoveZeroesTester.TestData),
        typeof(MoveZeroesTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int[] expected, int[] arr, bool _)
    {
        var res = MoveZeroesTester.StandardFunction(arr);
        CollectionAssert.AreEqual(expected, res);
    }
}
