namespace WebApplication1;

internal static class EndpointRouteBuilderExtensions
{
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

    public static IEndpointRouteBuilder MapWeatherForecast(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/weatherforecast", (HttpContext httpContext) =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
                .ToArray();
            return forecast;
        });

        return builder;
    }
}
