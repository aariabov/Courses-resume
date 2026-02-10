using Devpull.Controllers;
using Devpull.Course;
using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class AddTests
{
    [TestMethod]
    [DynamicData(nameof(AddTester.TestData), typeof(AddTester), DynamicDataSourceType.Method)]
    public void Test(int expected, int a, int b, bool _)
    {
        var res = AddTester.StandardFunction(a, b);
        Assert.AreEqual(expected, res);
    }
}
