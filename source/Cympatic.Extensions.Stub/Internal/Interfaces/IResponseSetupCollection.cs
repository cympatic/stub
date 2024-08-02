using Cympatic.Extensions.Stub.Models;
using Microsoft.AspNetCore.Http;

namespace Cympatic.Extensions.Stub.Internal.Interfaces;

internal interface IResponseSetupCollection
{
    IEnumerable<ResponseSetup> All();

    void AddOrUpdate(IEnumerable<ResponseSetup> responseModels);

    IEnumerable<ResponseSetup> Find(string httpMethod, string path, IQueryCollection query);

    ResponseSetup? GetById(Guid id);

    void Clear();

    bool Remove(ResponseSetup item);
}