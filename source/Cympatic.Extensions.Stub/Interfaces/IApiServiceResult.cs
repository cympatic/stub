using System.Net;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Stub.Interfaces;

public interface IApiServiceResult
{
    HttpStatusCode StatusCode { get; }

    string? ReasonPhrase { get; }

    HttpResponseHeaders? ResponseHeaders { get; }

    HttpContentHeaders? ContentHeaders { get; }

    string? Content { get; }

    bool IsSuccessStatusCode { get; }

    Task InitializeAsync(HttpResponseMessage response, CancellationToken cancellationToken = default);
}
