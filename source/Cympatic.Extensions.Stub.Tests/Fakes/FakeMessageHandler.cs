using System.Net;
using System.Text.Json;

namespace Cympatic.Extensions.Stub.Tests.Fakes;

public class FakeMessageHandler(HttpStatusCode expectedStatusCode, object? expectedReturnValue, string? expectedUrlPartial) : HttpMessageHandler
{
    private readonly IList<HttpRequestMessage> _calls = [];

    public FakeMessageHandler()
        : this(HttpStatusCode.OK, null, null)
    { }

    public FakeMessageHandler(object? expectedReturnValue)
        : this(HttpStatusCode.OK, expectedReturnValue, null)
    { }

    public FakeMessageHandler(HttpStatusCode expectedStatusCode, string? expectedUrlPartial)
        : this(expectedStatusCode, null, expectedUrlPartial)
    { }

    public FakeMessageHandler(object? expectedReturnValue, string? expectedUrlPartial)
        : this(HttpStatusCode.OK, expectedReturnValue, expectedUrlPartial)
    { }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _calls.Add(request);

        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(expectedUrlPartial) || (request.RequestUri?.AbsoluteUri.Contains(expectedUrlPartial, StringComparison.OrdinalIgnoreCase) ?? false))
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = expectedStatusCode,
                Content = GetContent()
            });
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
    }

    public int CallCount(string uri) => _calls.Count(request => request.RequestUri?.AbsoluteUri.Contains(uri, StringComparison.OrdinalIgnoreCase) ?? false);

    private StringContent? GetContent()
    {
        if (expectedReturnValue == null)
        {
            return null;
        }

        var returnValue = expectedReturnValue is string value
            ? value
            : JsonSerializer.Serialize(expectedReturnValue);

        return new StringContent(returnValue);
    }
}
