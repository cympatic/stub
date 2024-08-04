using Cympatic.Extensions.Stub.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cympatic.Extensions.Stub;

public sealed class StubServer : IDisposable
{
    private IHost? _host;

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

    public Uri BaseAddressStub => BaseAddress.Append("stub");

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

    private static IHost CreateHost()
    {
        var app = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .AddStubServer()
            .UseLocalhost()
            .UseStubServer()
            .AddApiService<SetupResponseApiService>()
            .AddApiService<ReceivedRequestApiService>();

        return app.Start();
    }
}
