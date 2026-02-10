using Devpull.Controllers;
using Devpull.Course;
using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class JoinTests
{
    [TestMethod]
    [DynamicData(nameof(JoinTester.TestData), typeof(JoinTester), DynamicDataSourceType.Method)]
    public void Test(string expected, int[] a, bool _)
    {
        var res = JoinTester.StandardFunction(a);
        Assert.AreEqual(expected, res);
    }
}
