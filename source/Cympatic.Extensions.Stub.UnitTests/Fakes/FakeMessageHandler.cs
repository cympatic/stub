using System.Net;
using System.Text.Json;

namespace Cympatic.Extensions.Stub.UnitTests.Fakes;

public class FakeMessageHandler : HttpMessageHandler
{
    private readonly IList<HttpRequestMessage> _calls = [];

    public string? ExpectedUrlPartial { get; set; }

    public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;

    public object? Response { get; set; }

    public IEnumerable<HttpRequestMessage> Calls(string uri) => _calls.Where(request => request.RequestUri?.AbsoluteUri.Contains(uri, StringComparison.OrdinalIgnoreCase) ?? false);

    public int CallCount(string uri) => _calls.Count(request => request.RequestUri?.AbsoluteUri.Contains(uri, StringComparison.OrdinalIgnoreCase) ?? false);

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _calls.Add(request);

        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(ExpectedUrlPartial) || (request.RequestUri?.AbsoluteUri.Contains(ExpectedUrlPartial, StringComparison.OrdinalIgnoreCase) ?? false))
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = ResponseStatusCode,
                Content = GetResponse()
            });
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
    }

    private StringContent? GetResponse()
    {
        if (Response == null)
        {
            return null;
        }

        var returnValue = Response is string value
            ? value
            : JsonSerializer.Serialize(Response);

        return new StringContent(returnValue);
    }
}
