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
            SetClientStubIdentifierValue(ClientStub, identifierValue);
        }

        public void SetClientStubIdentifierValue(IClientStub clientStub, string identifierValue)
        {
            SetClientStub(clientStub);

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { ClientStub.IdentifierHeaderName, new StringValues(identifierValue) } 
            };
            InternalHttpClient.DefaultRequestHeaders.AddRange(headers);
        }

        public Task AddOrUpdateAsync(ResponseModel model)
        {
            return AddOrUpdateAsync(new List<ResponseModel> { model });
        }

        public async Task AddOrUpdateAsync(IEnumerable<ResponseModel> models)
        {
            EnsureClientStubValid(ClientStub);

            EnsureHeadersValid(ClientStub);

            if (models == null)
            {
                throw new ArgumentNullException(nameof(models));
            }

            var uri = InternalHttpClient.BaseAddress
                .Append("setupresponse", ClientStub.Name, "addorupdate");

            using var response = await InternalHttpClient.PostAsync(uri, new StringContent(JsonSerializer.Serialize(models), Encoding.Default, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<ResponseModel>> GetAsync()
        {
            EnsureClientStubValid(ClientStub);

            EnsureHeadersValid(ClientStub);

            var uri = InternalHttpClient.BaseAddress
                .Append("setupresponse", ClientStub.Name, "getall");

            using var response = await InternalHttpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var models = await JsonSerializer.DeserializeAsync<List<ResponseModel>>(stream);

            return models;
        }

        public async Task RemoveAsync()
        {
            EnsureClientStubValid(ClientStub);

            EnsureHeadersValid(ClientStub);

            var uri = InternalHttpClient.BaseAddress
                .Append("setupresponse", ClientStub.Name, "remove");

            using var response = await InternalHttpClient.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
        }
    }
}
