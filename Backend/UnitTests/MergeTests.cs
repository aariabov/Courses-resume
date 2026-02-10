using ExerciseTester.Testers;

namespace UnitTests;

[TestClass]
public class MergeTests
{
    [TestMethod]
    [DynamicData(
        nameof(MergeTester.TestData),
        typeof(MergeTester),
        DynamicDataSourceType.Method
    )]
    public void Test(int[] expected, int[] nums1, int m, int[] nums2, int n)
    {
        var res = MergeTester.StandardFunction(nums1, m, nums2, n);
        CollectionAssert.AreEqual(expected, res);
    }
}
