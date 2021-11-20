using Cympatic.Extensions.Http.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Cympatic.Extensions.Http.Services
{
    public abstract class ApiService<TResult> : ApiService
        where TResult : IApiServiceResult, new()
    {
        protected ApiService(HttpClient httpClient) : base(httpClient)
        { }

        protected async Task<TResult> DeleteAsync(Uri uri)
        {
            return await DeleteAsync(uri, null);
        }

        protected async Task<TResult> DeleteAsync(Uri uri, HttpHeaders headers)
        {
            return await DeleteAsync(uri, headers, null);
        }

        protected async Task<TResult> DeleteAsync(Uri uri, HttpHeaders headers, object payload)
        {
            return await SendAsync<TResult>(HttpMethod.Delete, uri, headers, payload);
        }

        protected async Task<TResult> PutAsync(Uri uri)
        {
            return await PutAsync(uri, null);
        }

        protected async Task<TResult> PutAsync(Uri uri, HttpHeaders headers)
        {
            return await PutAsync(uri, headers, null);
        }

        protected async Task<TResult> PutAsync(Uri uri, HttpHeaders headers, object payload)
        {
            return await SendAsync<TResult>(HttpMethod.Put, uri, headers, payload);
        }

        protected async Task<TResult> PostAsync(Uri uri)
        {
            return await PostAsync(uri, null);
        }

        protected async Task<TResult> PostAsync(Uri uri, HttpHeaders headers)
        {
            return await PostAsync(uri, headers, null);
        }

        protected async Task<TResult> PostAsync(Uri uri, object payload)
        {
            return await PostAsync(uri, null, payload);
        }

        protected async Task<TResult> PostAsync(Uri uri, HttpHeaders headers, object payload)
        {
            return await SendAsync<TResult>(HttpMethod.Post, uri, headers, payload);
        }

        protected async Task<TResult> GetAsync(Uri uri)
        {
            return await GetAsync(uri, null);
        }

        protected async Task<TResult> GetAsync(Uri uri, HttpHeaders headers)
        {
            return await SendAsync<TResult>(HttpMethod.Get, uri, headers, null);
        }
    }
}
