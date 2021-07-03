using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Http
{
    public static class HttpHeadersExtensions
    {
        public static void AddRange(this HttpHeaders headers, IEnumerable<KeyValuePair<string, IEnumerable<string>>> range)
        {
            if (range != null)
            {
                foreach (var (key, value) in range)
                {
                    if (headers.TryGetValues(key, out var values))
                    {
                        headers.Remove(key);
                    }
                    headers.Add(key, values == null
                        ? value
                        : value.Union(values).ToArray());
                }
            }
        }

        public static void Merge(this HttpHeaders headers, IEnumerable<KeyValuePair<string, IEnumerable<string>>> range)
        {
            if (range != null)
            {
                foreach (var (key, value) in range)
                {
                    if (headers.TryGetValues(key, out var _))
                    {
                        headers.Remove(key);
                    }
                    headers.Add(key, value);
                }
            }
        }
    }
}
