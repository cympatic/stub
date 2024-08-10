using Cympatic.Extensions.Stub.Interfaces;
using System.Net;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Stub.Services.Results;

public class ApiServiceResult : IApiServiceResult
{
    public HttpStatusCode StatusCode { get; private set; }

    public string? ReasonPhrase { get; private set; }

    public HttpResponseHeaders? ResponseHeaders { get; private set; }

    public HttpContentHeaders? ContentHeaders { get; private set; }

    public string? Content { get; private set; }

    public bool IsSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode <= 299;

    public virtual async Task InitializeAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        StatusCode = response.StatusCode;
        ReasonPhrase = response.ReasonPhrase;
        ResponseHeaders = response.Headers;
        Content = string.Empty;

        if (response.Content is not null)
        {
            ContentHeaders = response.Content.Headers;
            Content = await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }

    public void EnsureSuccessStatusCode()
    {
        if (!IsSuccessStatusCode)
        {
            throw new HttpRequestException(ReasonPhrase, default, StatusCode);
        }
    }
}
