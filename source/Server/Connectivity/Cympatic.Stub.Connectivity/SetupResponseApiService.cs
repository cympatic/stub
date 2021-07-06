using Cympatic.Extensions.Http;
using Cympatic.Stub.Connectivity.Interfaces;
using Cympatic.Stub.Connectivity.Models;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cympatic.Stub.Connectivity
{
    public class SetupResponseApiService : StubApiService
    {
        public SetupResponseApiService(HttpClient httpClient) : base(httpClient)
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
            InternalHttpClient.DefaultRequestHeaders.AddRange(headers);
        }

        public Task AddOrUpdateAsync(ResponseModel model)
        {
            return AddOrUpdateAsync(new List<ResponseModel> { model });
        }

        public Task AddOrUpdateAsync(IEnumerable<ResponseModel> models)
        {
            return AddOrUpdateAsync(DefaultClientStub, models);
        }

        public Task AddOrUpdateAsync(IClientStub clientStub, ResponseModel model)
        {
            return AddOrUpdateAsync(clientStub, new List<ResponseModel> { model });
        }

        public async Task AddOrUpdateAsync(IClientStub clientStub, IEnumerable<ResponseModel> models)
        {
            EnsureClientStubValid(clientStub);

            EnsureHeadersValid(clientStub);

            if (models == null)
            {
                throw new ArgumentNullException(nameof(models));
            }

            var uri = InternalHttpClient.BaseAddress
                .Append("setupresponse", clientStub.Name, "addorupdate");

            using var response = await InternalHttpClient.PostAsync(uri, new StringContent(JsonSerializer.Serialize(models), Encoding.Default, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        public Task<IEnumerable<ResponseModel>> GetAsync()
        {
            return GetAsync(DefaultClientStub);
        }

        public async Task<IEnumerable<ResponseModel>> GetAsync(IClientStub clientStub)
        {
            EnsureClientStubValid(clientStub);

            EnsureHeadersValid(clientStub);

            var uri = InternalHttpClient.BaseAddress
                .Append("setupresponse", clientStub.Name, "getall");

            using var response = await InternalHttpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var models = await JsonSerializer.DeserializeAsync<List<ResponseModel>>(stream);

            return models;
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
                .Append("setupresponse", clientStub.Name, "remove");

            using var response = await InternalHttpClient.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
        }
    }
}
