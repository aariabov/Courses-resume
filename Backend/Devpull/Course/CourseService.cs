using Devpull.Exercises;
using Devpull.Exercises.Models;

namespace Devpull.Course;

public class CourseService
{
    private readonly CourseRepository _courseRepository;
    private readonly ExerciseService _exerciseService;

    public CourseService(CourseRepository courseRepository, ExerciseService exerciseService)
    {
        _courseRepository = courseRepository;
        _exerciseService = exerciseService;
    }

    public Task<CourseRegistryRecord[]> GetAll(CancellationToken cancellationToken)
    {
        return _courseRepository.GetAll(cancellationToken);
    }

    public async Task<CourseDto> GetByUrl(
        CourseParam courseParam,
        CancellationToken cancellationToken
    )
    {
        var c = await _courseRepository.GetCourseByUrl(courseParam.CourseUrl, cancellationToken);

        return new CourseDto
        {
            Id = c.Id.ToString(),
            Name = c.Name,
            Url = c.Url,
            Description = c.Description,
            Steps = c.Steps
                .OrderBy(s => s.Ord)
                .Select(
                    s =>
                        new StepDto
                        {
                            Id = s.Id.ToString(),
                            Name = s.Name,
                            Url = s.Url,
                            ShortDescription = s.ShortDescription,
                            Description = s.Description,
                            Lessons = s.Lessons
                                .OrderBy(l => l.Ord)
                                .Select(
                                    l =>
                                        new LessonDto
                                        {
                                            Id = l.Id.ToString(),
                                            Name =
                                                l.Exercise != null ? l.Exercise.ShortName : l.Name!,
                                            Url = l.Exercise != null ? l.Exercise.Url : l.Url!,
                                            Content =
                                                l.Exercise != null
                                                && l.Exercise.Url == courseParam.LessonUrl
                                                    ? $"# {l.Exercise.ShortName}"
                                                    : (
                                                        l.Url == courseParam.LessonUrl
                                                            ? l.Content
                                                            : null
                                                    ),
                                            Exercise =
                                                l.Exercise != null
                                                && l.Exercise.Url == courseParam.LessonUrl
                                                    ? new ExerciseDto
                                                    {
                                                        Id = l.Exercise.Id,
                                                        Description = l.Exercise.Description,
                                                        LevelId = l.Exercise.ExerciseLevelId,
                                                        Number = l.Exercise.Number,
                                                        Level = l.Exercise.ExerciseLevel.Name,
                                                        ShortName = l.Exercise.ShortName,
                                                        Url = l.Exercise.Url,
                                                        Template = l.Exercise.Template,
                                                        IsAccepted = l.Exercise.IsAccepted,
                                                        Examples = l.Exercise.ExerciseExamples
                                                            .Select(
                                                                x =>
                                                                    new ExerciseExampleDto
                                                                    {
                                                                        Id = x.Id,
                                                                        Input = x.Input,
                                                                        Output = x.Output,
                                                                        Explanation = x.Explanation,
                                                                    }
                                                            )
                                                            .ToArray()
                                                    }
                                                    : null
                                        }
                                )
                                .ToArray(),
                        }
                )
                .ToArray(),
        };
    }
}
