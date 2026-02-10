namespace Devpull.DbModels;

public class ExerciseLevel
{
    public required int Id { get; init; }
    public required string Name { get; init; }

    public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}
