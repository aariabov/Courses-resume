namespace Devpull.DbModels;

public class Tag
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }

    // many-to-many
    public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}
