using Devpull.DbModels;
using IntegrationTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

[assembly: AssemblyFixture(typeof(DatabaseFixture))]

namespace IntegrationTests;

/// <summary>
/// Поднимает БД и АПИ, используется как singleton между всеми тестами
/// </summary>
public sealed class DatabaseFixture : IDisposable
{
    public CustomWebApplicationFactory Factory { get; }

    public HttpClient Client { get; }
    private bool _disposed;

    public DatabaseFixture()
    {
        Factory = new CustomWebApplicationFactory();
        Client = Factory.CreateClient();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureDeleted();
        db.Database.Migrate();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Client.Dispose();
            Factory.Dispose();
        }

        _disposed = true;
    }
}
