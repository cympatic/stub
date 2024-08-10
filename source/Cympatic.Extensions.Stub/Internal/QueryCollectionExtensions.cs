using Microsoft.AspNetCore.Http;

namespace Cympatic.Extensions.Stub.Internal;

internal static class QueryCollectionExtensions
{
    public static IDictionary<string, string> ToDictionary(this IQueryCollection query)
    {
        var dict = new Dictionary<string, string>();
        foreach (var (key, value) in query)
        {
            dict[key] = string.Join(",", value!);
        }

        return dict;
    }
}
