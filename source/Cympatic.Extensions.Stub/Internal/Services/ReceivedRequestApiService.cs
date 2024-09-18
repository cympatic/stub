using Cympatic.Extensions.Stub.Internal;
using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services.Results;
using System.Collections.Specialized;

namespace Cympatic.Extensions.Stub.Services;

internal class ReceivedRequestApiService(HttpClient httpClient) : ApiService(httpClient)
{
    public async Task<IEnumerable<ReceivedRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var uri = new Uri("received", UriKind.Relative);

        var apiResult = await GetAsync<ApiServiceResult<IEnumerable<ReceivedRequest>>>(uri, cancellationToken);
        apiResult.EnsureSuccessStatusCode();

        return apiResult.Value ?? [];
    }

    public async Task<IEnumerable<ReceivedRequest>> FindAsync(ReceivedRequestSearchParams searchParams, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(searchParams);

        var uri = new Uri("received", UriKind.Relative).Append("find");

        if (!string.IsNullOrWhiteSpace(searchParams.Path))
        {
            uri = uri.WithParameter("path", searchParams.Path);
        }

        var parameters = new NameValueCollection();
        searchParams.Query
            .Select((kvp, index) => new { kvp.Key, kvp.Value, Index = index })
            .ToList()
            .ForEach(item =>
            {
                parameters.Add($"query[{item.Index}].key", item.Key);
                parameters.Add($"query[{item.Index}].value", item.Value);
            });

        searchParams.HttpMethods
            .Select((method, index) => new { Method = method, Index = index })
            .ToList()
            .ForEach(item => parameters.Add($"httpmethods[{item.Index}]", item.Method));

        uri = uri.WithParameters(parameters);

        var apiResult = await GetAsync<ApiServiceResult<IEnumerable<ReceivedRequest>>>(uri, cancellationToken);
        apiResult.EnsureSuccessStatusCode();

        return apiResult.Value ?? [];
    }

    public async Task RemoveAllAsync(CancellationToken cancellationToken = default)
    {
        var uri = new Uri("received", UriKind.Relative).Append("clear");

        var apiResult = await DeleteAsync<ApiServiceResult>(uri, cancellationToken);
        apiResult.EnsureSuccessStatusCode();
    }
}
