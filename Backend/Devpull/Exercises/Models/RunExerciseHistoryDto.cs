using Devpull.Common;
using Devpull.Course;

namespace Devpull.Exercises.Models;

public class RunExerciseHistoryDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required DateTime Date { get; init; }
    public string FunctionName => Helpers.GetFunctionName(Code);
    public required FunctionTestingResultView Result { get; init; }
}
