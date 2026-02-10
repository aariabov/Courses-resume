using System.Net.Http.Headers;
using System.Text;
using Devpull.DbModels;
using FluentAssertions;
using IntegrationTests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

[assembly: AssemblyFixture(typeof(DatabaseFixture))]

namespace IntegrationTests;

public class TestBase : IDisposable
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private bool _disposed;
    protected AppDbContext _db { get; }
    protected IConfiguration _config { get; }

    public TestBase(DatabaseFixture fixture)
    {
        _client = fixture.Client;

        _scope = fixture.Factory.Services.CreateScope();
        _config = _scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    protected async Task<TResult> PostAsync<TResult>(
        string url,
        object? model,
        string? token = null
    )
    {
        using var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(url, UriKind.Relative);

        if (model is not null)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );
        }

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var resultStr = await response.Content.ReadAsStringAsync();
        if (typeof(TResult) == typeof(string))
        {
            return (TResult)(object)resultStr;
        }

        var result = JsonConvert.DeserializeObject<TResult>(resultStr);
        result.Should().NotBeNull();
        return result;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _db.Dispose();
                _scope.Dispose();
            }

            // No unmanaged resources to release
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
