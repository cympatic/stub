using Cympatic.Extensions.Stub.IntegrationTests.Servers;
using Cympatic.Extensions.Stub.IntegrationTests.Servers.Models;
using Cympatic.Extensions.Stub.IntegrationTests.Services;
using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Cympatic.Extensions.Stub.IntegrationTests.Fixtures;

public class StubServerFixture : IDisposable
{
    private readonly StubServer _stubServer = new();
    private readonly TestServer _testServer;
    private readonly TestApiService _testApiService;

    internal TestServer TestServer => _testServer;

    internal TestApiService TestApiService => _testApiService;

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
        _testApiService = new TestApiService(_stubServer.BaseAddressStub.ToString());
    }

    public void Dispose()
    {
        _stubServer?.Dispose();
        _testServer?.Dispose();
        _testApiService?.Dispose();

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

    public ResponseSetup GenerateResponseSetup()
    {
        var httpMethods = new List<string>();
        for (var i = 0; i < 3; i++)
        {
            httpMethods.Add(HttpMethodNames[Random.Shared.Next(HttpMethodNames.Length)]);
        }

        var httpStatusCodes = Enum.GetValues(typeof(HttpStatusCode));

        return new()
        {
            HttpMethods = httpMethods,
            ReturnStatusCode = (HttpStatusCode)httpStatusCodes.GetValue(Random.Shared.Next(httpStatusCodes.Length))!,
            Location = new Uri(Guid.NewGuid().ToString("N"), UriKind.Relative),
            DelayInMilliseconds = Random.Shared.Next(1000),
            Path = Guid.NewGuid().ToString("N"),
            Query = GenerateQueryParameters(),
            Headers = GenerateHeaders()
        };
    }

    public ReceivedRequest GenerateReceivedRequest() 
        => new($"/{Guid.NewGuid():N}", HttpMethodNames[Random.Shared.Next(HttpMethodNames.Length)].ToUpperInvariant(), GenerateQueryParameters(), GenerateHeaders(), string.Empty, false);

    private static Dictionary<string, IEnumerable<string?>> GenerateHeaders() 
        => new()
        {
            { Guid.NewGuid().ToString("N"), [ Guid.NewGuid().ToString("N") ] },
            { Guid.NewGuid().ToString("N"), [ Guid.NewGuid().ToString("N") ] }
        };

    private static Dictionary<string, string> GenerateQueryParameters() 
        => new()
        {
            { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
            { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") }
        };

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

    private readonly string[] HttpMethodNames =
    [
        "Delete",
        "Get",
        "Head",
        "Options",
        "Patch",
        "Post",
        "Put",
        "Trace"
    ];
}
