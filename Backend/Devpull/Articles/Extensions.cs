namespace Devpull.Articles;

public static class Extensions
{
    public static void AddArticle(this IServiceCollection services)
    {
        services.AddScoped<ArticleRepository>();
        services.AddScoped<ArticleService>();
    }
}
