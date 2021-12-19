using Cympatic.Extensions.Http.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Extensions.Http.Services
{
    public abstract class ApiService
    {
        protected HttpClient HttpClient { get; }
        protected JsonSerializerOptions SerializerOptions { get; set; }

        protected ApiService(HttpClient httpClient)
        {
            HttpClient = httpClient;
            SerializerOptions = null;
        }

        protected async Task<TResult> DeleteAsync<TResult>(Uri uri, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await DeleteAsync<TResult>(uri, null, cancellationToken);
        }

        protected async Task<TResult> DeleteAsync<TResult>(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await DeleteAsync<TResult>(uri, headers, null, cancellationToken);
        }

        protected async Task<TResult> DeleteAsync<TResult>(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await SendAsync<TResult>(HttpMethod.Delete, uri, headers, payload, cancellationToken);
        }

        protected async Task<TResult> PutAsync<TResult>(Uri uri, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await PutAsync<TResult>(uri, null, cancellationToken);
        }

        protected async Task<TResult> PutAsync<TResult>(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await PutAsync<TResult>(uri, headers, null, cancellationToken);
        }

        protected async Task<TResult> PutAsync<TResult>(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await SendAsync<TResult>(HttpMethod.Put, uri, headers, payload, cancellationToken);
        }

        protected async Task<TResult> PostAsync<TResult>(Uri uri, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await PostAsync<TResult>(uri, null, cancellationToken);
        }

        protected async Task<TResult> PostAsync<TResult>(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await PostAsync<TResult>(uri, headers, null, cancellationToken);
        }

        protected async Task<TResult> PostAsync<TResult>(Uri uri, object payload, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await PostAsync<TResult>(uri, null, payload, cancellationToken);
        }

        protected async Task<TResult> PostAsync<TResult>(Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await SendAsync<TResult>(HttpMethod.Post, uri, headers, payload, cancellationToken);
        }

        protected async Task<TResult> GetAsync<TResult>(Uri uri, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await GetAsync<TResult>(uri, null, cancellationToken);
        }

        protected async Task<TResult> GetAsync<TResult>(Uri uri, HttpHeaders headers, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            return await SendAsync<TResult>(HttpMethod.Get, uri, headers, null, cancellationToken);
        }

        protected Task<TResult> SendAsync<TResult>([NotNull] HttpMethod httpMethod, [NotNull] Uri uri, HttpHeaders headers, object payload, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            var absoluteUri = uri.IsAbsoluteUri ? uri : HttpClient.BaseAddress.Append(uri.ToString());
            var httpRequestMessage = new HttpRequestMessage(httpMethod, absoluteUri);

            if (headers != null)
            {
                headers.AddRange(httpRequestMessage.Headers);
            }

            if (payload != null)
            {
                var payloadString = payload as string ?? JsonSerializer.Serialize(payload, payload.GetType(), SerializerOptions);

                httpRequestMessage.Content = new StringContent(payloadString, Encoding.UTF8, "application/json");
            }

            return GetResultAsync<TResult>(httpRequestMessage, cancellationToken);
        }

        private async Task<TResult> GetResultAsync<TResult>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
            where TResult : IApiServiceResult, new()
        {
            using var response = await HttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            var result = new TResult();
            await result.InitializeAsync(response, cancellationToken);

            return result;
        }
    }
}
