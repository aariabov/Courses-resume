namespace Devpull.Course;

public class CourseRegistryRecord
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required string ShortDescription { get; set; }
    public required int StepsCount { get; set; }
    public required int LessonsCount { get; set; }
}
