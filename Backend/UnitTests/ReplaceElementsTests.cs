using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class ReplaceElementsTests
{
    [TestMethod]
    [DynamicData(
        nameof(ReplaceElementsTester.TestData),
        typeof(ReplaceElementsTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int[] expected, int[] arr, bool _)
    {
        var res = ReplaceElementsTester.StandardFunction(arr);
        CollectionAssert.AreEqual(expected, res);
    }
}
