using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services.Results;
using System.Collections.Specialized;

namespace Cympatic.Extensions.Stub.Services;

public class ReceivedRequestApiService(HttpClient httpClient) : ApiService(httpClient)
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

        var i = 0;
        foreach (var (key, value) in searchParams.Query)
        {
            uri = uri.WithParameters(new NameValueCollection{
                { $"query[{i}].key", key },
                { $"query[{i}].value", value }
            });
            i++;
        }

        i = 0;
        foreach (var method in searchParams.HttpMethods)
        {
            uri = uri.WithParameter($"httpmethods[{i}]", method);
            i++;
        }

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
