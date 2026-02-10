using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Devpull.Users;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        // builder.ConfigureServices(services =>
        // {
        //     // Пример удаления реального контекста базы данных (если используется Entity Framework)
        //     services.RemoveAll<ICodeGenerator>();
        //     services.AddScoped<ICodeGenerator, FakeCodeGenerator>();
        // });
    }
}
