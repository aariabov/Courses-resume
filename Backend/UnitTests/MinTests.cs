using Devpull.Controllers;
using Devpull.Course;
using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class MinTests
{
    [TestMethod]
    [DynamicData(nameof(MinTester.TestData), typeof(MinTester), DynamicDataSourceType.Method)]
    public void Test(int expected, int a, int b, bool _)
    {
        var res = MinTester.StandardFunction(a, b);
        Assert.AreEqual(expected, res);
    }
}
