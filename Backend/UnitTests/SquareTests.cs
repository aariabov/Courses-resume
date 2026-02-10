using Devpull.Controllers;
using Devpull.Course;
using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class SquareTests
{
    [TestMethod]
    [DynamicData(nameof(SquareTester.TestData), typeof(SquareTester), DynamicDataSourceType.Method)]
    public void Test(int expected, int a, bool _)
    {
        var res = SquareTester.StandardFunction(a);
        Assert.AreEqual(expected, res);
    }
}
