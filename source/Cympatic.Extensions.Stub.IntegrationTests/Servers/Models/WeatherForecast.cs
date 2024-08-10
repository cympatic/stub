namespace Cympatic.Extensions.Stub.IntegrationTests.Servers.Models;

public record WeatherForecast(Guid Id, DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
