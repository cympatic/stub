using Microsoft.AspNetCore.Http;

namespace Cympatic.Extensions.Stub.Models;

public class ReceivedRequestSearchParams(string? path, IDictionary<string, string> query, IList<string> httpMethods)
{
    public ReceivedRequestSearchParams(string path) : this(path, new Dictionary<string, string>(), [])
    { }

    public ReceivedRequestSearchParams(string path, IList<string> httpMethods) : this(path, new Dictionary<string, string>(), httpMethods)
    { }

    public string? Path { get; set; } = path;

    public IDictionary<string, string> Query { get; set; } = query;

    public IList<string> HttpMethods { get; set; } = httpMethods;

    public static ValueTask<ReceivedRequestSearchParams?> BindAsync(HttpContext context)
    {
        var path = context.Request.Query["path"];
        
        var query = new Dictionary<string, string>();
        var i = 0;
        while (true)
        {
            var key = context.Request.Query[$"query[{i}].key"];
            var value = context.Request.Query[$"query[{i}].value"];
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            { 
                break;
            }

            query.Add(key!, value!);

            i++;
        }

        var httpMethods = new List<string>();
        i = 0;
        while (true)
        {
            var httpMethod = context.Request.Query[$"httpMethods[{i}]"];
            if (string.IsNullOrEmpty(httpMethod))
            {
                break;
            }

            httpMethods.Add(httpMethod!);

            i++;
        }

        return new ValueTask<ReceivedRequestSearchParams?>(new ReceivedRequestSearchParams(path, query, httpMethods));
    }
}
