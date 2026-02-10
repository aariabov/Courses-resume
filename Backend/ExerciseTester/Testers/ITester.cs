using Common;

namespace ExerciseTester.Testers;

public interface ITester
{
    Task<FunctionTestingResult> TestClientCode(string code);
    static abstract IEnumerable<object[]> TestData();
}
