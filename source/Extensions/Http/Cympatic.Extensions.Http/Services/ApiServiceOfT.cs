using Cympatic.Extensions.Http.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Extensions.Http.Services
{
    public abstract class ApiService<TResult> : ApiService
        where TResult : IApiServiceResult, new()
    {
        protected ApiService(HttpClient httpClient) : base(httpClient)
        { }

        protected Task<TResult> DeleteAsync(Uri uri, CancellationToken cancellationToken = default) 
            => DeleteAsync(uri, null, cancellationToken);

        protected Task<TResult> DeleteAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default) 
            => DeleteAsync(uri, headers, null, cancellationToken);

        protected Task<TResult> DeleteAsync(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default) 
            => SendAsync<TResult>(HttpMethod.Delete, uri, headers, payload, cancellationToken);

        protected Task<TResult> PutAsync(Uri uri, CancellationToken cancellationToken = default) 
            => PutAsync(uri, null, cancellationToken);

        protected Task<TResult> PutAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default) 
            => PutAsync(uri, headers, null, cancellationToken);

        protected Task<TResult> PutAsync(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default) 
            => SendAsync<TResult>(HttpMethod.Put, uri, headers, payload, cancellationToken);

        protected Task<TResult> PostAsync(Uri uri, CancellationToken cancellationToken = default) 
            => PostAsync(uri, null, cancellationToken);

        protected Task<TResult> PostAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default) 
            => PostAsync(uri, headers, null, cancellationToken);

        protected Task<TResult> PostAsync(Uri uri, object payload, CancellationToken cancellationToken = default) 
            => PostAsync(uri, null, payload, cancellationToken);

        protected Task<TResult> PostAsync(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default) 
            => SendAsync<TResult>(HttpMethod.Post, uri, headers, payload, cancellationToken);

        protected Task<TResult> GetAsync(Uri uri, CancellationToken cancellationToken = default) 
            => GetAsync(uri, null, cancellationToken);

        protected Task<TResult> GetAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default) 
            => SendAsync<TResult>(HttpMethod.Get, uri, headers, null, cancellationToken);
    }
}
