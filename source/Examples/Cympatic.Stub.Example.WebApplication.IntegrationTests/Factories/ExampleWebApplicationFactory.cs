using Cympatic.Extensions.Stub;
using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Cympatic.Stub.Example.WebApplication.IntegrationTests.Factories;

public class ExampleWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> 
    where TProgram : class
{
    private readonly StubServer _stubServer;
    private readonly SetupResponseApiService _setupResponseApiService;
    private readonly ReceivedRequestApiService _receivedRequestApiService;

    public ExampleWebApplicationFactory()
    {
        _stubServer = new();
        _setupResponseApiService = _stubServer.CreateApiService<SetupResponseApiService>();
        _receivedRequestApiService = _stubServer.CreateApiService<ReceivedRequestApiService>();
    }

    public Task<ResponseSetup> AddResponseSetupAsync(ResponseSetup responseSetup, CancellationToken cancellationToken = default)
        => _setupResponseApiService.AddAsync(responseSetup, cancellationToken);

    public Task AddResponsesSetupAsync(IEnumerable<ResponseSetup> responseSetups, CancellationToken cancellationToken = default)
        => _setupResponseApiService.AddAsync(responseSetups, cancellationToken);

    public Task ClearResponsesSetupAsync(CancellationToken cancellationToken = default)
        => _setupResponseApiService.RemoveAllAsync(cancellationToken);

    public Task<IEnumerable<ReceivedRequest>> FindReceivedRequestsAsync(ReceivedRequestSearchParams searchParams, CancellationToken cancellationToken = default)
        => _receivedRequestApiService.FindAsync(searchParams, cancellationToken);

    public Task ClearReceivedRequestsAsync(CancellationToken cancellationToken = default)
        => _receivedRequestApiService.RemoveAllAsync(cancellationToken);

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _stubServer.Dispose();
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            context.Configuration["ExternalApi"] = _stubServer.BaseAddressStub.ToString();
        });

        return base.CreateHost(builder);
    }
}
