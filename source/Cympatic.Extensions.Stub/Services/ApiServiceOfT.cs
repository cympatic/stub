using Cympatic.Extensions.Stub.Interfaces;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Stub.Services;

public abstract class ApiService<TResult>(HttpClient httpClient) : ApiService(httpClient)
    where TResult : IApiServiceResult, new()
{
    protected Task<TResult> DeleteAsync(Uri uri, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Delete, uri, null, null, cancellationToken);

    protected Task<TResult> DeleteAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Delete, uri, headers, null, cancellationToken);

    protected Task<TResult> DeleteAsync(Uri uri, HttpHeaders? headers, object payload, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Delete, uri, headers, payload, cancellationToken);

    protected Task<TResult> PutAsync(Uri uri, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Put, uri, null, null, cancellationToken);

    protected Task<TResult> PutAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Put, uri, headers, null, cancellationToken);

    protected Task<TResult> PutAsync(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Put, uri, headers, payload, cancellationToken);

    protected Task<TResult> PostAsync(Uri uri, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Post, uri, null, null, cancellationToken);

    protected Task<TResult> PostAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Post, uri, headers, null, cancellationToken);

    protected Task<TResult> PostAsync(Uri uri, object payload, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Post, uri, null, payload, cancellationToken);

    protected Task<TResult> PostAsync(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Post, uri, headers, payload, cancellationToken);

    protected Task<TResult> GetAsync(Uri uri, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Get, uri, null, null, cancellationToken);

    protected Task<TResult> GetAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        => SendAsync<TResult>(HttpMethod.Get, uri, headers, null, cancellationToken);
}
