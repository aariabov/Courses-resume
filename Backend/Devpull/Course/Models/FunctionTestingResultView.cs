using Common;
using Devpull.Exercises;

namespace Devpull.Course;

public enum TestStatusView
{
    Success = 1,
    Fail = 2,
    Error = 3,
}

public class TestErrorView
{
    public required string Error { get; init; }
    public required string[] Parameters { get; init; }
}

public class FunctionTestingResultView
{
    public TestStatusView Status { get; }
    public TestErrorView[] TestErrors { get; } = [];
    
    public string? Error { get; set; }

    public FunctionTestingResultView(FunctionTestingResult result)
    {
        Status = result.Status.GetPublicStatus();

        if (Status == TestStatusView.Fail)
        {
            TestErrors = result.Results
                .Where(r => (r.Error != null || r.FailMsg != null) && r.IsPublic)
                .Select(r => new TestErrorView{ Error = (r.Error ?? r.FailMsg)!, Parameters = r.Parameters})
                .Take(3)
                .ToArray();
        }

        if (Status == TestStatusView.Error)
        {
            Error = result.CompileError + result.Error;
        }
    }
}