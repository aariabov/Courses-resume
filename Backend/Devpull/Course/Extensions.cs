namespace Devpull.Course;

public static class Extensions
{
    public static void AddCourse(this IServiceCollection services)
    {
        services.AddScoped<CourseService>();
        services.AddScoped<CourseRepository>();
    }
}
