using Microsoft.AspNetCore.Http;

namespace Cympatic.Extensions.Stub.Internal;

internal static class HeaderDictionaryExtensions
{
    public static IDictionary<string, IEnumerable<string?>> ToDictionary(this IHeaderDictionary headers)
    {
        var list = headers.Select(header => new KeyValuePair<string, IEnumerable<string?>>(header.Key, header.Value.Select(value => value).ToList()));

        return new Dictionary<string, IEnumerable<string?>>(list.ToList());
    }

    public static void AddRange(this IHeaderDictionary headers, IDictionary<string, IEnumerable<string?>>? range)
    {
        if (range is not null)
        {
            foreach (var (key, value) in range)
            {
                if (headers.TryGetValue(key, out var values))
                {
                    headers.Remove(key);
                }
                headers.Append(key, value.Union(values).ToArray());
            }
        }
    }
}
