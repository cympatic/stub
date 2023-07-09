using Cympatic.Extensions.Http;
using Cympatic.Stub.Connectivity.Interfaces;
using Cympatic.Stub.Connectivity.Models;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Stub.Connectivity;

public class SetupResponseApiService : StubApiService
{
    public SetupResponseApiService(HttpClient httpClient) : base(httpClient)
    { }

    public void SetIdentifierValue(string identifierValue)
    {
        SetClientStubIdentifierValue(ClientStub, identifierValue, true);
    }

    public void SetClientStubIdentifierValue(IClientStub clientStub, string identifierValue, bool useIdentificationHeader)
    {
        SetClientStub(clientStub);

        if (useIdentificationHeader)
        {
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { ClientStub.IdentifierHeaderName, new StringValues(identifierValue) }
            };
            InternalHttpClient.DefaultRequestHeaders.AddRange(headers);
        }
    }

    public Task AddOrUpdateAsync(ResponseModel model, CancellationToken cancellationToken = default)
    {
        return AddOrUpdateAsync(new List<ResponseModel> { model }, cancellationToken);
    }

    public async Task AddOrUpdateAsync(IEnumerable<ResponseModel> models, CancellationToken cancellationToken = default)
    {
        EnsureClientStubValid(ClientStub);

        EnsureHeadersValid(ClientStub);

        if (models == null)
        {
            throw new ArgumentNullException(nameof(models));
        }

        var uri = InternalHttpClient.BaseAddress
            .Append("setupresponse", ClientStub.Name, "addorupdate");

        using var response = await InternalHttpClient.PostAsync(uri, new StringContent(JsonSerializer.Serialize(models), Encoding.Default, "application/json"), cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<ResponseModel>> GetAsync(CancellationToken cancellationToken = default)
    {
        EnsureClientStubValid(ClientStub);

        EnsureHeadersValid(ClientStub);

        var uri = InternalHttpClient.BaseAddress
            .Append("setupresponse", ClientStub.Name, "getall");

        using var response = await InternalHttpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var models = await JsonSerializer.DeserializeAsync<List<ResponseModel>>(stream, default(JsonSerializerOptions), cancellationToken);

        return models;
    }

    public async Task RemoveAsync(CancellationToken cancellationToken = default)
    {
        EnsureClientStubValid(ClientStub);

        EnsureHeadersValid(ClientStub);

        var uri = InternalHttpClient.BaseAddress
            .Append("setupresponse", ClientStub.Name, "remove");

        using var response = await InternalHttpClient.DeleteAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
