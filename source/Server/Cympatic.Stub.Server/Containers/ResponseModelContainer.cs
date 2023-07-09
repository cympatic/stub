using Cympatic.Extensions.Http;
using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Cympatic.Stub.Server.Containers;

public class ResponseModelContainer : ModelContainer<ResponseModel>
{
    private readonly ILogger _logger;

    public ResponseModelContainer(ILogger<ResponseModelContainer> logger) : base(logger)
    {
        _logger = logger;
    }

    public virtual void AddOrUpdate(string identifierValue, IEnumerable<ResponseModel> responseModels)
    {
        AddOrUpdate(identifierValue, responseModels, (items, model) => AddOrUpdateModel(items, model));
    }

    public virtual ResponseModel FindResult(string identifierValue, string httpMethod, string path, IQueryCollection query)
    {
        _logger.LogDebug("{type}.FindResult for identifier: '{identifier}', httpMethod: '{httpMethod}', path: '{path}', and query:\r\n{@query}",
            GetType().Name, identifierValue, httpMethod, path, query);

        return Get(identifierValue)
            .Where(model => model.IsMatching(httpMethod, path, query.ToDictionary())).FirstOrDefault();
    }

    private static void AddOrUpdateModel(HashSet<ResponseModel> items, ResponseModel responseModel)
    {
        var foundItems = items.Where(model => model.AreRequestParamsEqual(responseModel.HttpMethods, responseModel.Path, responseModel.Query));
        foundItems.ToList().ForEach(item => items.Remove(item));

        items.Add(responseModel);
    }
}
