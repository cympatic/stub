using Cympatic.Extensions.Stub.Internal.Interfaces;
using Cympatic.Extensions.Stub.Internal.Utilities;
using Cympatic.Extensions.Stub.Models;
using Microsoft.AspNetCore.Http;

namespace Cympatic.Extensions.Stub.Internal.Collections;

internal sealed class ResponseSetupCollection : AutomaticExpireCollection<ResponseSetup>, IResponseSetupCollection
{
    public void AddOrUpdate(IEnumerable<ResponseSetup> responseModels)
        => AddOrUpdate(responseModels, AddOrUpdateItem);

    public ResponseSetup? GetById(Guid id)
    {
        return Find(item => item.Id == id).FirstOrDefault();
    }

    public IEnumerable<ResponseSetup> Find(string httpMethod, string path, IQueryCollection query)
    {
        return Find(item => IsMatching(item, httpMethod, path, query.ToDictionary()));
    }

    private void AddOrUpdateItem(IEnumerable<ResponseSetup> items, ResponseSetup newItem)
    {
        var foundItems = items.Where(item => AreRequestParamsEqual(item, newItem.HttpMethods, newItem.Path, newItem.Query));
        foundItems.ToList().ForEach(item => Remove(item));

        Add(newItem);
    }

    private static bool AreRequestParamsEqual(ResponseSetup item, IList<string> httpMethods, string path, IDictionary<string, string> query)
    {
        var modelHttpMethods = item.HttpMethods ?? [];
        httpMethods ??= [];

        return
            modelHttpMethods.All(value => httpMethods.Contains(value, StringComparer.OrdinalIgnoreCase)) &&
            SearchableStubbedHttpItemUtility.ComparePath(item.Path, path) &&
            SearchableStubbedHttpItemUtility.CompareQuery(item.Query, query);
    }

    private static bool IsMatching(ResponseSetup item, string httpMethod, string path, IDictionary<string, string> query)
    {
        return SearchableStubbedHttpItemUtility.IsMatching(item.HttpMethods, httpMethod, path, item.Path, query, item.Query);
    }
}
