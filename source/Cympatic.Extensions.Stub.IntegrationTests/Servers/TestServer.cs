using Cympatic.Extensions.Stub.IntegrationTests.Servers.Extensions;
using Cympatic.Extensions.Stub.IntegrationTests.Servers.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Stub.IntegrationTests.Servers;

internal sealed class TestServer : IDisposable
{
    private IHost? _host;
    private Uri? _baseAddressExternalApi;

    public IHost Host
    {
        get
        {
            return _host ??= CreateHost();
        }
    }

    public Uri BaseAddress
    {
        get
        {
            return new Uri(Host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First());
        }
    }

    public void Dispose()
    {
        ResetHost();

        GC.SuppressFinalize(this);
    }

    public void ResetHost()
    {
        _host?.Dispose();
        _host = null;
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
        if (_baseAddressExternalApi is null)
        {
            throw new InvalidOperationException("BaseAddress to External Api hasn't be set!");
        }

        var app = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder.ConfigureServices((services) =>
                {
                    services.AddRouting();

                    services.AddHttpClient<ExternalApiService>((serviceProvider, config) =>
                    {
                        config.BaseAddress = _baseAddressExternalApi;
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
