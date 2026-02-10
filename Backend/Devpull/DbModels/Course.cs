using System;
using System.Collections.Generic;

namespace Devpull.DbModels;

public partial class Course
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ShortDescription { get; set; } = null!;

    public virtual ICollection<Step> Steps { get; set; } = new List<Step>();
}
