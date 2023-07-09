using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Cympatic.Stub.Server.Containers;

public class RequestModelContainer : ModelContainer<RequestModel>
{
    private readonly ILogger _logger;

    public RequestModelContainer(ILogger<RequestModelContainer> logger) : base(logger)
    {
        _logger = logger;
    }

    public virtual RequestModel AddRequest(
        string identifierValue,
        string path,
        IDictionary<string, string> query,
        string httpMethod,
        IDictionary<string, IEnumerable<string>> headers,
        string body,
        bool responseFound)
    {
        _logger.LogDebug("{type}.AddRequest for identifier: '{identifier}', path: '{path}', httpMethod: '{httpMethod}', query:\r\n{@query}\r\n, and {body}",
            GetType().Name, identifierValue, path, httpMethod, query, body);

        var requestModel = new RequestModel
        {
            Path = path,
            Query = query,
            HttpMethod = httpMethod,
            Headers = headers,
            Body = body,
            ResponseFound = responseFound
        };

        AddModel(identifierValue, requestModel);

        return requestModel;
    }

    public virtual IEnumerable<RequestModel> Find(string identifierValue, string path, IDictionary<string, string> query, IList<string> httpMethods)
    {
        _logger.LogDebug("{type}.FindResult for identifier: '{identifier}', path: '{path}', httpMethods: '{httpMethods}', and query:\r\n{@query}",
            GetType().Name, identifierValue, path, httpMethods, query);

        return Get(identifierValue)
            .Where(model => model.ResponseFound && model.IsMatching(httpMethods, path, query)).ToArray();
    }
}
