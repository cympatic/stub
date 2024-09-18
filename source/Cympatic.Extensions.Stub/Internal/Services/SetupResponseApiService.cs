using Cympatic.Extensions.Stub.Internal;
using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services.Results;

namespace Cympatic.Extensions.Stub.Services;

internal class SetupResponseApiService(HttpClient httpClient) : ApiService(httpClient)
{
    public async Task<IEnumerable<ResponseSetup>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var uri = new Uri("setup", UriKind.Relative);

        var apiResult = await GetAsync<ApiServiceResult<IEnumerable<ResponseSetup>>>(uri, cancellationToken);
        apiResult.EnsureSuccessStatusCode();

        return apiResult.Value ?? [];
    }

    public async Task<ResponseSetup> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var uri = new Uri("setup", UriKind.Relative).Append(id.ToString("N"));

        var apiResult = await GetAsync<ApiServiceResult<ResponseSetup>>(uri, cancellationToken);
        apiResult.EnsureSuccessStatusCode();

        return apiResult.Value ?? new();
    }

    public async Task<ResponseSetup> AddAsync(ResponseSetup responseSetup, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(responseSetup);

        var uri = new Uri("setup", UriKind.Relative).Append("response");

        var apiResult = await PostAsync<ApiServiceResult<ResponseSetup>>(uri, responseSetup, cancellationToken);
        apiResult.EnsureSuccessStatusCode();

        return apiResult.Value ?? new();
    }

    public async Task AddAsync(IEnumerable<ResponseSetup> responseSetups, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(responseSetups);

        var uri = new Uri("setup", UriKind.Relative).Append("responses");

        var apiResult = await PostAsync<ApiServiceResult>(uri, responseSetups, cancellationToken);
        apiResult.EnsureSuccessStatusCode();
    }

    public Task RemoveAsync(ResponseSetup responseSetup, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(responseSetup);

        return RemoveAsync(responseSetup.Id, cancellationToken);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var uri = new Uri("setup", UriKind.Relative).Append("remove", id.ToString("N"));

        var apiResult = await DeleteAsync<ApiServiceResult>(uri, cancellationToken);
        apiResult.EnsureSuccessStatusCode();
    }

    public async Task RemoveAllAsync(CancellationToken cancellationToken = default)
    {
        var uri = new Uri("setup", UriKind.Relative).Append("clear");

        var apiResult = await DeleteAsync<ApiServiceResult>(uri, cancellationToken);
        apiResult.EnsureSuccessStatusCode();
    }
}
