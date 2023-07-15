using Cympatic.Extensions.Http;
using Cympatic.Stub.Connectivity.Interfaces;
using Cympatic.Stub.Connectivity.Internal;
using Cympatic.Stub.Connectivity.Models;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Stub.Connectivity;

public class SetupClientApiService : StubApiService
{
    public SetupClientApiService(HttpClient httpClient) : base(httpClient)
    { }

    public async Task<IClientStub> SetupAsync()
    {
        await SetupAsync(ClientStub);
        return ClientStub;
    }

    public async Task<IClientStub> SetupAsync(string clientName, string identifierHeader, CancellationToken cancellationToken = default)
    {
        var client = new ClientStub(clientName, identifierHeader);
        await SetupAsync(client, cancellationToken);

        return client;
    }

    public virtual async Task SetupAsync(IClientStub clientStub, CancellationToken cancellationToken = default)
    {
        EnsureClientStubValid(clientStub);

        var uri = InternalHttpClient.BaseAddress
            .Append("setupclient", clientStub.Name, "add")
            .WithParameters(new NameValueCollection {
                { "identifierHeader", clientStub.IdentifierHeaderName },
                { "responseTtlInMinutes", clientStub.ResponseTtlInMinutes.ToString() },
                { "requestTtlInMinutes", clientStub.RequestTtlInMinutes.ToString() }
            });

        using var response = await InternalHttpClient.PostAsync(uri, null, cancellationToken);
        response.EnsureSuccessStatusCode();

        SetClientStub(clientStub);
    }

    public async Task<IClientStub> GetClientAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        var uri = InternalHttpClient.BaseAddress
            .Append("setupclient", name, "getclient");

        using var response = await InternalHttpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var client = await JsonSerializer.DeserializeAsync<ClientModel>(stream, default(JsonSerializerOptions), cancellationToken);

            return new ClientStub(client.Name, client.IdentifierHeaderName);
        }

        return default;
    }

    public Task RemoveAsync(CancellationToken cancellationToken = default)
    {
        return RemoveAsync(ClientStub, cancellationToken);
    }

    public virtual async Task RemoveAsync(IClientStub clientStub, CancellationToken cancellationToken = default)
    {
        EnsureClientStubValid(clientStub);

        var uri = InternalHttpClient.BaseAddress
            .Append("setupclient", clientStub.Name, "remove");

        using var response = await InternalHttpClient.DeleteAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        ResetClientStub();
    }
}