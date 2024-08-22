using Cympatic.Extensions.Stub;
using Cympatic.Extensions.Stub.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Cympatic.Stub.Example.WebApplication.IntegrationTests.Factories;

public class ExampleWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> 
    where TProgram : class
{
    private readonly StubServer _stubServer;

    public ExampleWebApplicationFactory()
    {
        _stubServer = new();
    }

    public Task<ResponseSetup> AddResponseSetupAsync(ResponseSetup responseSetup, CancellationToken cancellationToken = default)
        => _stubServer.AddResponseSetupAsync(responseSetup, cancellationToken);

    public Task AddResponsesSetupAsync(IEnumerable<ResponseSetup> responseSetups, CancellationToken cancellationToken = default)
        => _stubServer.AddResponsesSetupAsync(responseSetups, cancellationToken);

    public Task ClearResponsesSetupAsync(CancellationToken cancellationToken = default)
        => _stubServer.ClearResponsesSetupAsync(cancellationToken);

    public Task<IEnumerable<ReceivedRequest>> FindReceivedRequestsAsync(ReceivedRequestSearchParams searchParams, CancellationToken cancellationToken = default)
        => _stubServer.FindReceivedRequestsAsync(searchParams, cancellationToken);

    public Task ClearReceivedRequestsAsync(CancellationToken cancellationToken = default)
        => _stubServer.ClearReceivedRequestsAsync(cancellationToken);

    public void Clear()
    {
        Task.Run(() => ClearAsync()).GetAwaiter().GetResult(); 
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _stubServer.ClearResponsesSetupAsync(cancellationToken);
        await _stubServer.ClearReceivedRequestsAsync(cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _stubServer.Dispose();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            context.Configuration["ExternalApi"] = _stubServer.BaseAddressStub.ToString();
        });

        base.ConfigureWebHost(builder);
    }
}
