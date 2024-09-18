using Cympatic.Extensions.Stub.Internal;
using Cympatic.Extensions.Stub.Internal.Services.Results;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Cympatic.Extensions.Stub.Services;

internal abstract class ApiService(HttpClient httpClient)
{
    protected HttpClient HttpClient { get; } = httpClient;
    protected JsonSerializerOptions? SerializerOptions { get; set; } = null;

    protected Task<TResult> DeleteAsync<TResult>(Uri uri, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Delete, uri, null, null, cancellationToken);

    protected Task<TResult> DeleteAsync<TResult>(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Delete, uri, headers, null, cancellationToken);

    protected Task<TResult> DeleteAsync<TResult>(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Delete, uri, headers, payload, cancellationToken);

    protected Task<TResult> PutAsync<TResult>(Uri uri, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Put, uri, null, null, cancellationToken);

    protected Task<TResult> PutAsync<TResult>(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Put, uri, headers, null, cancellationToken);

    protected Task<TResult> PutAsync<TResult>(Uri uri, HttpHeaders? headers, object payload, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Put, uri, headers, payload, cancellationToken);

    protected Task<TResult> PostAsync<TResult>(Uri uri, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Post, uri, null, null, cancellationToken);

    protected Task<TResult> PostAsync<TResult>(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Post, uri, headers, null, cancellationToken);

    protected Task<TResult> PostAsync<TResult>(Uri uri, object payload, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
            => SendAsync<TResult>(HttpMethod.Post, uri, null, payload, cancellationToken);

    protected Task<TResult> PostAsync<TResult>(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Post, uri, headers, payload, cancellationToken);

    protected Task<TResult> GetAsync<TResult>(Uri uri, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Get, uri, null, null, cancellationToken);

    protected Task<TResult> GetAsync<TResult>(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
        => SendAsync<TResult>(HttpMethod.Get, uri, headers, null, cancellationToken);

    protected Task<TResult> SendAsync<TResult>(HttpMethod httpMethod, Uri uri, HttpHeaders? headers, object? payload, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
    {
        ArgumentNullException.ThrowIfNull(httpMethod);
        ArgumentNullException.ThrowIfNull(uri);

        var absoluteUri = uri.IsAbsoluteUri ? uri : HttpClient.BaseAddress?.Append(uri.ToString());
        var httpRequestMessage = new HttpRequestMessage(httpMethod, absoluteUri);

        httpRequestMessage.Headers.AddRange(headers);

        if (payload != null)
        {
            var payloadString = payload as string ?? JsonSerializer.Serialize(payload, payload.GetType(), SerializerOptions);

            httpRequestMessage.Content = new StringContent(payloadString, Encoding.UTF8, "application/json");
        }

        return GetResultAsync<TResult>(httpRequestMessage, cancellationToken);
    }

    private async Task<TResult> GetResultAsync<TResult>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
        where TResult : ApiServiceResult, new()
    {
        using var response = await HttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        var result = new TResult();
        await result.InitializeAsync(response, cancellationToken);

        return result;
    }
}
