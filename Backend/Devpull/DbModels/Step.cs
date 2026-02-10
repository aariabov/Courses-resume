using System;
using System.Collections.Generic;

namespace Devpull.DbModels;

public partial class Step
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ShortDescription { get; set; } = null!;

    public string Url { get; set; } = null!;

    public Guid CourseId { get; set; }

    public int Ord { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
