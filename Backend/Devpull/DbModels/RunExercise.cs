using System.Text.Json;
using Devpull.Controllers;
using Devpull.Course;

namespace Devpull.DbModels;

public class RunExercise
{
    public required Guid Id { get; init; }
    public required string UserId { get; init; }
    public required Guid ExerciseId { get; init; }
    public required string Code { get; init; }
    public required JsonDocument Result { get; init; }
    public required DateTime Date { get; init; }

    public virtual AppUser User { get; set; } = null!;
    public virtual Exercise Exercise { get; set; } = null!;
}
