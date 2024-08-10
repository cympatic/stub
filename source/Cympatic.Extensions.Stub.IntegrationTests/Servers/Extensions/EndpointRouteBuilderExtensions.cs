using Cympatic.Extensions.Stub.IntegrationTests.Servers.Models;
using Cympatic.Extensions.Stub.IntegrationTests.Servers.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cympatic.Extensions.Stub.IntegrationTests.Servers.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapWeatherForecast(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/weatherforecast", async (ExternalApiService apiService, CancellationToken cancellationToken) 
            => await apiService.GetAllAsync(cancellationToken));

        builder.MapGet("/weatherforecast/{id}", async (Guid id, ExternalApiService apiService, CancellationToken cancellationToken) 
            => await apiService.GetByIdAsync(id, cancellationToken));

        builder.MapPost("/weatherforecast", async (WeatherForecast forecast, ExternalApiService apiService, CancellationToken cancellationToken) 
            => await apiService.AddAsync(forecast, cancellationToken));

        builder.MapDelete("/weatherforecast/{id}", async (Guid id, ExternalApiService apiService, CancellationToken cancellationToken) 
            => await apiService.RemoveAsync(id, cancellationToken));

        return builder;
    }
}
