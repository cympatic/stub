using Cympatic.Extensions.Stub.IntegrationTests.Servers.Models;
using Cympatic.Extensions.Stub.Services;
using Cympatic.Extensions.Stub.Services.Results;

namespace Cympatic.Extensions.Stub.IntegrationTests.Servers.Services;

internal class WeatherForecastApiService(HttpClient httpClient) : ApiService(httpClient)
{
    public async Task<IEnumerable<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var uri = new Uri("weatherforecast", UriKind.Relative);

        var result = await GetAsync<ApiServiceResult<IEnumerable<WeatherForecast>>>(uri, cancellationToken);
        result.EnsureSuccessStatusCode();

        return result.Value ?? [];
    }

    public async Task<WeatherForecast?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var uri = new Uri("weatherforecast", UriKind.Relative).Append(id.ToString("N"));

        var result = await GetAsync<ApiServiceResult<WeatherForecast>>(uri, cancellationToken);
        result.EnsureSuccessStatusCode();

        return result.Value;
    }

    public async Task<WeatherForecast?> AddAsync(WeatherForecast forecast, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(forecast);

        var uri = new Uri("weatherforecast", UriKind.Relative);

        var result = await PostAsync<ApiServiceResult<WeatherForecast>>(uri, forecast, cancellationToken);
        result.EnsureSuccessStatusCode();

        return result.Value;
    }

    public Task RemoveAsync(WeatherForecast forecast, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(forecast);

        return RemoveAsync(forecast.Id, cancellationToken);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var uri = new Uri("weatherforecast", UriKind.Relative).Append(id.ToString("N"));

        var result = await DeleteAsync<ApiServiceResult<WeatherForecast>>(uri, cancellationToken);
        result.EnsureSuccessStatusCode();
    }
}
