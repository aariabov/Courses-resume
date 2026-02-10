using Devpull.Controllers;
using Devpull.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Devpull.Course;

public class CourseRepository
{
    private readonly AppDbContext _db;

    public CourseRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CourseRegistryRecord[]> GetAll(CancellationToken cancellationToken)
    {
        return await _db.Courses
            .Select(
                a =>
                    new CourseRegistryRecord
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Url = a.Url,
                        ShortDescription = a.ShortDescription,
                        StepsCount = a.Steps.Count(),
                        LessonsCount = a.Steps.SelectMany(s => s.Lessons).Count()
                    }
            )
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Devpull.DbModels.Course> GetCourseByUrl(
        string url,
        CancellationToken cancellationToken
    )
    {
        return await _db.Courses
                .Include(c => c.Steps.OrderBy(s => s.Ord))
                .ThenInclude(s => s.Lessons.OrderBy(l => l.Ord))
                .ThenInclude(l => l.Exercise)
                .ThenInclude(e => e!.ExerciseLevel)
                .Include(c => c.Steps.OrderBy(s => s.Ord))
                .ThenInclude(s => s.Lessons.OrderBy(l => l.Ord))
                .ThenInclude(l => l.Exercise)
                .ThenInclude(e => e!.ExerciseExamples)
                .FirstOrDefaultAsync(c => c.Url == url, cancellationToken)
            ?? throw new NotFoundException(url);
    }
}
