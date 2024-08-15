using Cympatic.Extensions.Stub.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cympatic.Extensions.Stub;

public sealed class StubServer(Func<StubServerOptions>? configureOptions) : IDisposable
{
    private IHost? _host;

    public IHost Host
    {
        get
        {
            return _host ??= CreateHost();
        }
    }

    public Uri BaseAddress => new(Host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First());

    public Uri BaseAddressStub => BaseAddress.Append("stub");

    public StubServer(bool useSsl = false) : this(() => new StubServerOptions { UseSsl = useSsl })
    { }

    public void Dispose()
    {
        ResetHost();

        GC.SuppressFinalize(this);
    }

    public TApiService CreateApiService<TApiService>()
        where TApiService : ApiService
    {
        return (TApiService)CreateApiService(typeof(TApiService));
    }

    public ApiService CreateApiService(Type type)
    {
        if (!type.IsAssignableTo(typeof(ApiService)))
        {
            throw new InvalidOperationException($"Type: {type.Name} doesn't derive from class 'ApiService'");
        }

        return (ApiService)Host.Services.GetRequiredService(type);
    }

    public void ResetHost()
    {
        _host?.Dispose();
        _host = null;
    }

    private IHost CreateHost()
    {
        var options = configureOptions?.Invoke() ?? new StubServerOptions();
        var useSsl = options.UseSsl && options.ServerCertificateSelector is null;

        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .AddStubServer()
            .UseStubServer();

        hostBuilder = useSsl
            ? hostBuilder
                .UseLocalhost(useSsl)
                .AddApiService<SetupResponseApiService>(useSsl)
                .AddApiService<ReceivedRequestApiService>(useSsl)
            : hostBuilder
                .UseLocalhost(options.ServerCertificateSelector)
                .AddApiService<SetupResponseApiService>(options.ConfigureHttpClientHandler)
                .AddApiService<ReceivedRequestApiService>(options.ConfigureHttpClientHandler);

        return hostBuilder.Start();
    }
}
