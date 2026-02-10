using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class ValidMountainArrayTests
{
    [TestMethod]
    [DynamicData(
        nameof(ValidMountainArrayTester.TestData),
        typeof(ValidMountainArrayTester),
        DynamicDataSourceType.Method
    )]
    public void Test(bool expected, int[] arr, bool _)
    {
        var res = ValidMountainArrayTester.StandardFunction(arr);
        Assert.AreEqual(expected, res);
    }
}
