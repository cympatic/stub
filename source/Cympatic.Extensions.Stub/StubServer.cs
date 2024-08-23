using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cympatic.Extensions.Stub;

public sealed class StubServer : IDisposable
{
    private readonly Func<StubServerOptions>? _configureOptions;

    private IHost _host;

    public IHost Host => _host;

    public Uri BaseAddress => new(_host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First());

    public Uri BaseAddressStub => BaseAddress.Append("stub");

    public StubServer(bool useSsl = false) : this(() => new StubServerOptions { UseSsl = useSsl })
    { }

    public StubServer(Func<StubServerOptions>? configureOptions)
    {
        _configureOptions = configureOptions;
        _host = CreateHost();
    }

    public void Dispose()
    {
        _host?.Dispose();

        GC.SuppressFinalize(this);
    }

    public Task<ResponseSetup> AddResponseSetupAsync(ResponseSetup responseSetup, CancellationToken cancellationToken = default)
        => GetSetupResponseApiService().AddAsync(responseSetup, cancellationToken);

    public Task AddResponsesSetupAsync(IEnumerable<ResponseSetup> responseSetups, CancellationToken cancellationToken = default)
        => GetSetupResponseApiService().AddAsync(responseSetups, cancellationToken);

    public Task ClearResponsesSetupAsync(CancellationToken cancellationToken = default)
        => GetSetupResponseApiService().RemoveAllAsync(cancellationToken);

    public Task<IEnumerable<ReceivedRequest>> GetAllReceivedRequestsAsync(CancellationToken cancellationToken = default)
        => GetReceivedRequestApiService().GetAllAsync(cancellationToken);

    public Task<IEnumerable<ReceivedRequest>> FindReceivedRequestsAsync(ReceivedRequestSearchParams searchParams, CancellationToken cancellationToken = default)
        => GetReceivedRequestApiService().FindAsync(searchParams, cancellationToken);

    public Task ClearReceivedRequestsAsync(CancellationToken cancellationToken = default)
        => GetReceivedRequestApiService().RemoveAllAsync(cancellationToken);

    [Obsolete("Use methods 'AddResponseSetupAsync', 'AddResponsesSetupAsync', 'ClearResponsesSetupAsync', 'FindReceivedRequestsAsync', or 'ClearReceivedRequestsAsync' instead of CreateApiService and using the ApiService")]
    public TApiService CreateApiService<TApiService>()
        where TApiService : ApiService
    {
        return (TApiService)CreateApiService(typeof(TApiService));
    }

    [Obsolete("Use methods 'AddResponseSetupAsync', 'AddResponsesSetupAsync', 'ClearResponsesSetupAsync', 'FindReceivedRequestsAsync', or 'ClearReceivedRequestsAsync' instead of CreateApiService and using the ApiService")]
    public ApiService CreateApiService(Type type)
    {
        if (!type.IsAssignableTo(typeof(ApiService)))
        {
            throw new InvalidOperationException($"Type: {type.Name} doesn't derive from class 'ApiService'");
        }

        return (ApiService)_host.Services.GetRequiredService(type);
    }

    public void ResetHost()
    {
        _host?.Dispose();
        _host = CreateHost();
    }

    private IHost CreateHost()
    {
        var options = _configureOptions?.Invoke() ?? new StubServerOptions();
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

    private SetupResponseApiService GetSetupResponseApiService()
        => _host.Services.GetRequiredService<SetupResponseApiService>();

    private ReceivedRequestApiService GetReceivedRequestApiService()
        => _host.Services.GetRequiredService<ReceivedRequestApiService>();
}
