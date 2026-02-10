using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class TwoSumTests
{
    [TestMethod]
    [DynamicData(
        nameof(TwoSumTester.TestData),
        typeof(TwoSumTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int[] expected, int[] numbers, int target, bool _)
    {
        var res = TwoSumTester.StandardFunction(numbers, target);
        CollectionAssert.AreEqual(expected, res);
    }
}
