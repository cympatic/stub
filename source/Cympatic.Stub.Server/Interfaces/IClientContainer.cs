using Cympatic.Stub.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Cympatic.Stub.Server.Interfaces
{
    public interface IClientContainer
    {
        ClientModel Add(string identifierHeaderName, int responseTtlInMinutes, int requestTtlInMinutes);

        ClientModel GetClient();

        IEnumerable<ClientModel> GetClients();

        void AddOrUpdateResponses(IEnumerable<ResponseModel> responses);

        RequestModel AddRequest(string path, IDictionary<string, string> query, string httpMethod, IDictionary<string, IEnumerable<string>> headers, string body, bool responseFound);

        ResponseModel FindResult(string httpMethod, string path, IQueryCollection query);

        IEnumerable<RequestModel> GetRequests();

        IEnumerable<ResponseModel> GetResponses();

        void Remove();

        void RemoveRequests();

        void RemoveResponses();

        IEnumerable<RequestModel> SearchRequests(RequestSearchModel searchModel);
    }
}