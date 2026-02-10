namespace Devpull.Exercises.Models;

public class ExerciseRegistryRecord
{
    public required Guid Id { get; init; }
    public required int LevelId { get; init; }
    public required int Number { get; init; }
    public required string Level { get; init; }
    public required string ShortName { get; init; }
    public required string Url { get; init; }
    public required bool IsAccepted { get; init; }
}
