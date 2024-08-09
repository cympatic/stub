using Cympatic.Stub.Example.WebApplication.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<ExternalApiService>((serviceProvider, config) =>
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var baseAddressExternalApi = configuration.GetValue<string>("ExternalApi");

        if (string.IsNullOrWhiteSpace(baseAddressExternalApi))
        {
            throw new ArgumentException($"{nameof(baseAddressExternalApi)} must be provided");
        }

        config.BaseAddress = new Uri(baseAddressExternalApi);
        config.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    });


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapGet("/weatherforecast", async (ExternalApiService apiService, CancellationToken cancellationToken)
    => await apiService.GetAllAsync(cancellationToken));

app.MapGet("/weatherforecast/{id}", async (Guid id, ExternalApiService apiService, CancellationToken cancellationToken)
    => await apiService.GetByIdAsync(id, cancellationToken));

app.MapPost("/weatherforecast", async (WeatherForecast forecast, ExternalApiService apiService, CancellationToken cancellationToken)
    => await apiService.AddAsync(forecast, cancellationToken));

app.MapDelete("/weatherforecast/{id}", async (Guid id, ExternalApiService apiService, CancellationToken cancellationToken)
    => await apiService.RemoveAsync(id, cancellationToken));

app.Run();

public partial class Program { }
