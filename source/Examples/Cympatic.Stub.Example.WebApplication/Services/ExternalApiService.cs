using System.Text.Json;

namespace Cympatic.Stub.Example.WebApplication.Services;

public record WeatherForecast(Guid Id, DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal class ExternalApiService(HttpClient httpClient) 
{
    public async Task<IEnumerable<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var uri = new Uri($"{httpClient.BaseAddress!}/external/api/weatherforecast");

        using var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<IList<WeatherForecast>>(body) ?? [];
    }

    public async Task<WeatherForecast?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var uri = new Uri($"{httpClient.BaseAddress!}/external/api/weatherforecast/{id:N}");

        using var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<WeatherForecast>(body);
    }

    public async Task<WeatherForecast?> AddAsync(WeatherForecast forecast, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(forecast);

        var uri = new Uri($"{httpClient.BaseAddress!}/external/api/weatherforecast");

        using var response = await httpClient.PostAsync(uri, new StringContent(JsonSerializer.Serialize(forecast)), cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<WeatherForecast>(body);
    }

    public Task RemoveAsync(WeatherForecast forecast, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(forecast);

        return RemoveAsync(forecast.Id, cancellationToken);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var uri = new Uri($"{httpClient.BaseAddress!}/external/api/weatherforecast/{id:N}");

        using var response = await httpClient.DeleteAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();

    }
}
