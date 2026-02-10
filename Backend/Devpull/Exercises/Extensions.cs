using Common;
using Devpull.Course;

namespace Devpull.Exercises;

public static class Extensions
{
    public static void AddExercise(this IServiceCollection services)
    {
        services.AddScoped<ExerciseService>();
        services.AddScoped<ExerciseRepository>();
    }

    public static TestStatusView GetPublicStatus(this TestStatus status)
    {
        return status switch
        {
            TestStatus.Success => TestStatusView.Success,
            TestStatus.CompileError
            or TestStatus.TimeoutError
            or TestStatus.CommonError
                => TestStatusView.Error,
            _ => TestStatusView.Fail
        };
    }
}
