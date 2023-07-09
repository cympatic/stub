using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Cympatic.Extensions.Http;

public static class QueryCollectionExtensions
{
    public static IDictionary<string, string> ToDictionary(this IQueryCollection query)
    {
        var dict = new Dictionary<string, string>();
        foreach (var (key, value) in query)
        {
            dict[key] = string.Join(",", value);
        }

        return dict;
    }
}
