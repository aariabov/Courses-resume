using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class MaxArrayElementTests
{
    [TestMethod]
    [DynamicData(nameof(MaxArrayElementTester.TestData), typeof(MaxArrayElementTester), DynamicDataSourceType.Method)]
    public void Test(int expected, int[] arr, bool _)
    {
        var res = MaxArrayElementTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}



