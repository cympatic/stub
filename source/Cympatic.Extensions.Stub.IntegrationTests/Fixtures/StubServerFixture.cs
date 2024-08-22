using Cympatic.Extensions.Stub.IntegrationTests.Servers;
using Cympatic.Extensions.Stub.IntegrationTests.Servers.Models;
using Cympatic.Extensions.Stub.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cympatic.Extensions.Stub.IntegrationTests.Fixtures;

public class StubServerFixture : IDisposable
{
    private readonly StubServer _stubServer = new();
    private readonly TestServer _testServer;

    internal TestServer TestServer => _testServer;

    internal SetupResponseApiService SetupResponseApiService
        => _stubServer.Host.Services.GetRequiredService<SetupResponseApiService>();

    internal ReceivedRequestApiService ReceivedRequestApiService
        => _stubServer.Host.Services.GetRequiredService<ReceivedRequestApiService>();

    public StubServer StubServer => _stubServer;

    public StubServerFixture()
    {
        _stubServer = new();
        _testServer = new();

        _testServer.SetBaseAddressExternalApi(_stubServer.BaseAddressStub);
    }

    public void Dispose()
    {
        _stubServer?.Dispose();
        _testServer?.Dispose();

        GC.SuppressFinalize(this);
    }

    public void Clear()
    {
        Task.Run(() => ClearAsync()).GetAwaiter().GetResult();
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _stubServer.ClearResponsesSetupAsync(cancellationToken);
        await _stubServer.ClearReceivedRequestsAsync(cancellationToken);
    }

    public WeatherForecast GenerateWeatherForecast(int index = 0)
        => new(Guid.NewGuid(), DateTime.Now.Date.AddDays(index), Random.Shared.Next(-20, 55), summaries[Random.Shared.Next(summaries.Length)]);

    private readonly string[] summaries =
    [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    ];
}
