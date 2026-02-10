using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class IsEvenTests
{
    [TestMethod]
    [DynamicData(nameof(IsEvenTester.TestData), typeof(IsEvenTester), DynamicDataSourceType.Method)]
    public void Test(bool expected, int number, bool _)
    {
        var res = IsEvenTester.StandardFunction(number);
        Assert.AreEqual(expected, res);
    }
}



