using System;
using System.Collections.Generic;

namespace Devpull.DbModels;

public partial class Lesson
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Url { get; set; }

    public string? Content { get; set; }

    public Guid StepId { get; set; }

    public int Ord { get; set; }

    public virtual Step Step { get; set; } = null!;

    public Guid? ExerciseId { get; set; }

    public virtual Exercise? Exercise { get; set; }
}
