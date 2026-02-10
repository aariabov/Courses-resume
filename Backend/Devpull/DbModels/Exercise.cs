using Common;

namespace Devpull.DbModels;

public class Exercise
{
    public required Guid Id { get; init; }
    public required string ShortName { get; init; }
    public required string Description { get; init; }
    public required string Url { get; init; }
    public required string Template { get; init; }
    public required int ExerciseLevelId { get; set; }
    public int Number { get; set; }

    public virtual ExerciseLevel ExerciseLevel { get; set; } = null!;

    public virtual ICollection<RunExercise> RunExercises { get; set; } = new List<RunExercise>();
    public virtual ICollection<ExerciseExample> ExerciseExamples { get; set; } =
        new List<ExerciseExample>();
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    // many-to-many
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public bool IsAccepted
    {
        get
        {
            foreach (var runExercise in RunExercises)
            {
                var status = (TestStatus)
                    runExercise.Result.RootElement
                        .GetProperty(nameof(FunctionTestingResult.Status))
                        .GetInt32();
                if (status == TestStatus.Success)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
