using Cympatic.Extensions.Http;
using Cympatic.Stub.Abstractions.Models;
using Cympatic.Stub.Connectivity.Interfaces;
using Cympatic.Stub.Connectivity.Internal;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cympatic.Stub.Connectivity
{
    public class SetupClientApiService : StubApiService
    {
        public SetupClientApiService(HttpClient httpClient) : base(httpClient)
        { }

        public async Task<IClientStub> SetupAsync()
        {
            await SetupAsync(DefaultClientStub);
            return DefaultClientStub;
        }

        public async Task<IClientStub> SetupAsync(string clientName, string identifierHeader)
        {
            var client = new ClientStub(clientName, identifierHeader);
            await SetupAsync(client);

            return client;
        }

        public async Task SetupAsync(IClientStub clientStub)
        {
            EnsureClientStubValid(clientStub);

            var uri = InternalHttpClient.BaseAddress
                .Append("setupclient", clientStub.Name, "add")
                .WithParameters(new NameValueCollection {
                    { "identifierHeader", clientStub.IdentifierHeaderName },
                    { "responseTtlInMinutes", clientStub.ResponseTtlInMinutes.ToString() },
                    { "requestTtlInMinutes", clientStub.RequestTtlInMinutes.ToString() }
                });

            using var response = await InternalHttpClient.PostAsync(uri, null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IClientStub> GetClientAsync(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new ArgumentNullException(nameof(clientName));
            }

            var uri = InternalHttpClient.BaseAddress
                .Append("setupclient", clientName, "getclient");

            using var response = await InternalHttpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var client = await JsonSerializer.DeserializeAsync<ClientModel>(stream);

                return new ClientStub(client.Name, client.IdentifierHeaderName);
            }

            return default;
        }

        public Task RemoveAsync()
        {
            return RemoveAsync(DefaultClientStub);
        }

        public async Task RemoveAsync(IClientStub clientStub)
        {
            EnsureClientStubValid(clientStub);

            var uri = InternalHttpClient.BaseAddress
                .Append("setupclient", clientStub.Name, "remove");

            using var response = await InternalHttpClient.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
        }
    }
}