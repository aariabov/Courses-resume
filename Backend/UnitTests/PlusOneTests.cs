using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class PlusOneTests
{
    [TestMethod]
    [DynamicData(
        nameof(PlusOneTester.TestData),
        typeof(PlusOneTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int[] expected, int[] arr, bool _)
    {
        var res = PlusOneTester.StandardFunction(arr);
        CollectionAssert.AreEqual(expected, res);
    }
}
