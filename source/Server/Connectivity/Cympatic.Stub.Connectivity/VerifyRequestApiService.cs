using Cympatic.Extensions.Http;
using Cympatic.Stub.Connectivity.Interfaces;
using Cympatic.Stub.Connectivity.Models;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Stub.Connectivity;

public class VerifyRequestApiService : StubApiService
{
    public VerifyRequestApiService(HttpClient httpClient) : base(httpClient)
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

    public async Task<IEnumerable<RequestModel>> GetAsync(CancellationToken cancellationToken = default)
    {
        EnsureClientStubValid(ClientStub);

        EnsureHeadersValid(ClientStub);

        var uri = InternalHttpClient.BaseAddress
            .Append("verifyrequest", ClientStub.Name, "getall");

        using var response = await InternalHttpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<List<RequestModel>>(stream, default(JsonSerializerOptions), cancellationToken);
    }

    public async Task<IEnumerable<RequestModel>> SearchAsync(RequestSearchModel searchModel, CancellationToken cancellationToken = default)
    {
        EnsureClientStubValid(ClientStub);

        EnsureHeadersValid(ClientStub);

        if (searchModel == null)
        {
            throw new ArgumentNullException(nameof(searchModel));
        }

        var uri = InternalHttpClient.BaseAddress
            .Append("verifyrequest", ClientStub.Name, "search")
            .WithParameter("path", searchModel.Path);

        var i = 0;
        foreach (var (key, value) in searchModel.Query)
        {
            uri = uri.WithParameters(new NameValueCollection{
                { $"query[{i}].key", key },
                { $"query[{i}].value", value }
            });
            i++;
        }

        i = 0;
        foreach (var method in searchModel.HttpMethods)
        {
            uri = uri.WithParameter($"httpmethods[{i}]", method);
            i++;
        }

        using var response = await InternalHttpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = stream is object
            ? await JsonSerializer.DeserializeAsync<List<RequestModel>>(stream, default(JsonSerializerOptions), cancellationToken)
            : new List<RequestModel>();

        return result;
    }

    public async Task RemoveAsync(CancellationToken cancellationToken = default)
    {
        EnsureClientStubValid(ClientStub);

        EnsureHeadersValid(ClientStub);

        var uri = InternalHttpClient.BaseAddress
            .Append("verifyrequest", ClientStub.Name, "remove");

        using var response = await InternalHttpClient.DeleteAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
