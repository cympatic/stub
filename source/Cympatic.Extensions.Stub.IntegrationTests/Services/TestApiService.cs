using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Cympatic.Extensions.Stub.Internal;

namespace Cympatic.Extensions.Stub.IntegrationTests.Services;

internal class TestApiService : IDisposable
{
    private readonly HttpClient _httpClient;

    public TestApiService(string baseAddress)
    {
        if (string.IsNullOrWhiteSpace(baseAddress))
        {
            throw new ArgumentException($"{nameof(baseAddress)} must be provided");
        }

        _httpClient = new HttpClient(new HttpClientHandler
        {
            UseProxy = false,
            UseDefaultCredentials = true,
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        }, true)
        {
            BaseAddress = new Uri(baseAddress)
        };
        _httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void Dispose()
    {
        _httpClient?.Dispose();

        GC.SuppressFinalize(this);
    }

    public async Task<(HttpStatusCode statusCode, string? body)> SendAsync(Uri uri, HttpMethod httpMethod, IDictionary<string, IEnumerable<string?>>? headers, object? payload = default, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpMethod);
        ArgumentNullException.ThrowIfNull(uri);

        var httpRequestMessage = new HttpRequestMessage(httpMethod, _httpClient.BaseAddress?.Append(uri.ToString()));

        headers ??=new Dictionary<string, IEnumerable<string?>>();
        foreach (var (key, values) in headers)
        {
            httpRequestMessage.Headers.Add(key, values);
        }

        if (payload != null)
        {
            var payloadString = payload as string ?? JsonSerializer.Serialize(payload, payload.GetType());

            httpRequestMessage.Content = new StringContent(payloadString, Encoding.UTF8, "application/json");
        }

        using var response = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        return (response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
    }
}
