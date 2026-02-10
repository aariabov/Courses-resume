using Devpull.Common;
using Devpull.Exercises.Models;

namespace Devpull.Course;

public class CourseDto
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Url { get; init; }

    public required string Description { get; init; }

    public required StepDto[] Steps { get; init; } = [];
}

public class StepDto
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string ShortDescription { get; init; }

    public required string Url { get; init; }

    public required LessonDto[] Lessons { get; init; } = [];
}

public class LessonDto
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Url { get; init; }

    public string? Content { get; init; }
    public ExerciseDto? Exercise { get; set; }
}
