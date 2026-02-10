namespace Devpull.DbModels;

public class ExerciseExample
{
    public required Guid Id { get; init; }
    public required string Input { get; init; }
    public required string Output { get; init; }
    public required string Explanation { get; init; }

    public virtual Exercise Exercise { get; set; } = null!;
}
