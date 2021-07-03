using Cympatic.Extensions.Http;
using Cympatic.Stub.Abstractions.Models;
using Cympatic.Stub.Connectivity.Interfaces;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cympatic.Stub.Connectivity
{
    public class VerifyRequestApiService : StubApiService
    {
        public VerifyRequestApiService(HttpClient httpClient) : base(httpClient)
        { }

        public void SetIdentifierValue(string identifierValue)
        {
            SetIdentifierValue(DefaultClientStub, identifierValue);
        }

        public void SetIdentifierValue(IClientStub clientStub, string identifierValue)
        {
            EnsureClientStubValid(clientStub);

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { clientStub.IdentifierHeaderName, new StringValues(identifierValue) } 
            };
            InternalHttpClient.DefaultRequestHeaders.Merge(headers);
        }

        public Task<IEnumerable<RequestModel>> GetAsync()
        {
            return GetAsync(DefaultClientStub);
        }

        public async Task<IEnumerable<RequestModel>> GetAsync(IClientStub clientStub)
        {
            EnsureClientStubValid(clientStub);

            EnsureHeadersValid(clientStub);

            var uri = InternalHttpClient.BaseAddress
                .Append("verifyrequest", clientStub.Name, "getall");

            using var response = await InternalHttpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<RequestModel>>(stream);
        }

        public Task<IEnumerable<RequestModel>> SearchAsync(RequestSearchModel searchModel)
        {
            return SearchAsync(DefaultClientStub, searchModel);
        }

        public async Task<IEnumerable<RequestModel>> SearchAsync(IClientStub clientStub, RequestSearchModel searchModel)
        {
            EnsureClientStubValid(clientStub);

            EnsureHeadersValid(clientStub);

            if (searchModel == null)
            {
                throw new ArgumentNullException(nameof(searchModel));
            }

            var uri = InternalHttpClient.BaseAddress
                .Append("verifyrequest", clientStub.Name, "search")
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

            using var response = await InternalHttpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var result = stream is object
                ? await JsonSerializer.DeserializeAsync<List<RequestModel>>(stream)
                : new List<RequestModel>();

            return result;
        }

        public Task RemoveAsync()
        {
            return RemoveAsync(DefaultClientStub);
        }

        public async Task RemoveAsync(IClientStub clientStub)
        {
            EnsureClientStubValid(clientStub);

            EnsureHeadersValid(clientStub);

            var uri = InternalHttpClient.BaseAddress
                .Append("verifyrequest", clientStub.Name, "remove");

            using var response = await InternalHttpClient.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
        }
    }
}
