using Cympatic.Extensions.Stub;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text.Json;

namespace WebApplication1;

public class StubMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    private static readonly string[] summaries =
    [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    ];

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"4. Endpoint: {context.GetEndpoint()?.DisplayName ?? "(null)"}");

        var b = context.Request.Path.StartsWithSegments("/stub", StringComparison.OrdinalIgnoreCase);

        var arr = context.Request.Path.Value?.Split("/", StringSplitOptions.RemoveEmptyEntries) ?? [];

        var uri = new Uri("/", UriKind.Relative).Append(arr[1..]);
        var str = uri.ToString();

        object forecast = Enumerable
            .Range(1, 5)
            .Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
            .ToArray();

        context.Response.StatusCode = StatusCodes.Status201Created;
        await context.Response.WriteAsJsonAsync(forecast);

        await _next(context);
    }
}
