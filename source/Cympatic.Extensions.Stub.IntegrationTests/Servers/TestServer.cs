using Cympatic.Extensions.Stub.IntegrationTests.Servers.Extensions;
using Cympatic.Extensions.Stub.IntegrationTests.Servers.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Stub.IntegrationTests.Servers;

internal sealed class TestServer : IDisposable
{
    private const string DefaultBaseAddress = "http://localhost";

    private IHost _host;
    private Uri? _baseAddressExternalApi;

    public IHost Host => _host;

    public Uri BaseAddress => new(Host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First());

    public TestServer()
    {
        _host = CreateHost();
    }

    public void Dispose()
    {
        _host?.Dispose();

        GC.SuppressFinalize(this);
    }

    public void ResetHost()
    {
        _host?.Dispose();
        _host = CreateHost();
    }

    public void SetBaseAddressExternalApi(Uri baseAddress)
    {
        _baseAddressExternalApi = baseAddress;
        ResetHost();
    }

    public TestServerApiService CreateTestServerApiService()
    {
        return Host.Services.GetRequiredService<TestServerApiService>();
    }

    private IHost CreateHost()
    {
        var app = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder.ConfigureServices((context, services) =>
                {
                    context.Configuration["ExternalApi"] = _baseAddressExternalApi?.ToString();

                    services.AddRouting();

                    services.AddHttpClient<ExternalApiService>((serviceProvider, config) =>
                    {
                        var configuration = Host.Services.GetRequiredService<IConfiguration>();
                        var baseAddressExternalApi = configuration.GetValue<string>("ExternalApi");

                        if (string.IsNullOrWhiteSpace(baseAddressExternalApi))
                        {
                            throw new ArgumentException($"{nameof(baseAddressExternalApi)} must be provided");
                        }

                        config.BaseAddress = new Uri(baseAddressExternalApi);
                        config.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    })
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        return new HttpClientHandler
                        {
                            UseProxy = false,
                            UseDefaultCredentials = true,
                            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                        };
                    });
                });

                webHostBuilder.Configure(app =>
                {
                    app.UseRouting();

                    app.UseEndpoints(endpointBuilder =>
                    {
                        endpointBuilder.MapWeatherForecast();
                    });
                });
            })
            .UseLocalhost()
            .AddApiService<TestServerApiService>();

        return app.Start();
    }
}
