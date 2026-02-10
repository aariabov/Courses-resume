using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class ReverseArrayTests
{
    [TestMethod]
    [DynamicData(nameof(ReverseArrayTester.TestData), typeof(ReverseArrayTester), DynamicDataSourceType.Method)]
    public void Test(int[] expected, int[] arr, bool _)
    {
        var res = ReverseArrayTester.StandardFunction(arr);
        CollectionAssert.AreEqual(expected, res);
    }
}





