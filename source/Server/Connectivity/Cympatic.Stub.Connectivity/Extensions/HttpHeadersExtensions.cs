using System.Linq;
using System.Net.Http.Headers;

namespace Cympatic.Stub.Connectivity.Extensions
{
    public static class HttpHeadersExtensions
    {
        public static bool HasValidHeaders(this HttpHeaders headers, string identifierHeaderName)
        {
            return headers.TryGetValues("Accept", out var values) &&
                values.Any(value => value == "application/json") &&
                headers.TryGetValues(identifierHeaderName, out var _);
        }
    }
}
