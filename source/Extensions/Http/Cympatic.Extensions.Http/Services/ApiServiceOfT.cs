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

        protected async Task<TResult> DeleteAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            return await DeleteAsync(uri, null, cancellationToken);
        }

        protected async Task<TResult> DeleteAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        {
            return await DeleteAsync(uri, headers, null, cancellationToken);
        }

        protected async Task<TResult> DeleteAsync(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
        {
            return await SendAsync<TResult>(HttpMethod.Delete, uri, headers, payload, cancellationToken);
        }

        protected async Task<TResult> PutAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            return await PutAsync(uri, null, cancellationToken);
        }

        protected async Task<TResult> PutAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        {
            return await PutAsync(uri, headers, null, cancellationToken);
        }

        protected async Task<TResult> PutAsync(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
        {
            return await SendAsync<TResult>(HttpMethod.Put, uri, headers, payload, cancellationToken);
        }

        protected async Task<TResult> PostAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            return await PostAsync(uri, null, cancellationToken);
        }

        protected async Task<TResult> PostAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        {
            return await PostAsync(uri, headers, null, cancellationToken);
        }

        protected async Task<TResult> PostAsync(Uri uri, object payload, CancellationToken cancellationToken = default)
        {
            return await PostAsync(uri, null, payload, cancellationToken);
        }

        protected async Task<TResult> PostAsync(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
        {
            return await SendAsync<TResult>(HttpMethod.Post, uri, headers, payload, cancellationToken);
        }

        protected async Task<TResult> GetAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            return await GetAsync(uri, null, cancellationToken);
        }

        protected async Task<TResult> GetAsync(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
        {
            return await SendAsync<TResult>(HttpMethod.Get, uri, headers, null, cancellationToken);
        }
    }
}
