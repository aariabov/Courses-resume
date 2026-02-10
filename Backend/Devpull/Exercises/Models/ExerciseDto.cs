using System.Text.RegularExpressions;
using Devpull.Common;

namespace Devpull.Exercises.Models;

public class ExerciseDto
{
    public required Guid Id { get; init; }
    public required int LevelId { get; init; }
    public required int Number { get; init; }
    public required string Level { get; init; }
    public required string ShortName { get; init; }
    public required string Url { get; init; }
    public required string Template { get; init; }
    public required string Description { get; init; }
    public required bool IsAccepted { get; init; }
    public required ExerciseExampleDto[] Examples { get; init; }
    public string FunctionName => Helpers.GetFunctionName(Template);
}

public class ExerciseExampleDto
{
    public required Guid Id { get; init; }
    public required string Input { get; init; }
    public required string Output { get; init; }
    public required string Explanation { get; init; }
}
