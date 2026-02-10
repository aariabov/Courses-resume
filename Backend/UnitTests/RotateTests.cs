using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class RotateTests
{
    [TestMethod]
    [DynamicData(nameof(RotateTester.TestData), typeof(RotateTester), DynamicDataSourceType.Method)]
    public void Test(int[] expected, int[] arr, int k, bool _)
    {
        RotateTester.StandardFunction(arr, k);
        CollectionAssert.AreEqual(expected, arr);
    }
}
