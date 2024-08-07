using Cympatic.Extensions.Stub.Models;
using Cympatic.Stub.Example.WebApplication.IntegrationTests.Factories;
using Cympatic.Stub.Example.WebApplication.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;

namespace Cympatic.Stub.Example.WebApplication.IntegrationTests;

public class WeatherForecastTests : IClassFixture<ExampleWebApplicationFactory<Program>>
{
    private const int NumberOfItems = 10;

    private readonly ExampleWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public WeatherForecastTests(ExampleWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();

        _factory.ClearResponsesSetupAsync();
        _factory.ClearReceivedRequestsAsync();
    }

    [Fact]
    public async Task GetAllWeatherForecasts()
    {
        static IEnumerable<WeatherForecast> GetItems()
        {
            for (var i = 0; i < NumberOfItems; i++)
            {
                yield return GenerateWeatherForecast(i);
            }
        }

        // Arrange
        var expected = GetItems().ToList();
        var responseSetup = new ResponseSetup
        {
            Path = "/external/api/weatherforecast",
            HttpMethods = [HttpMethod.Get.ToString()],
            ReturnStatusCode = HttpStatusCode.OK,
            Response = expected
        };
        await _factory.AddResponseSetupAsync(responseSetup);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, string.Empty, true)
        };

        // Act
        var response = await _httpClient.GetAsync("/weatherforecast");

        // Assert
        var actual = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();

        var actualReceivedRequests = await _factory.FindReceivedRequestsAsync(new ReceivedRequestSearchParams("/external/api/weatherforecast", [HttpMethod.Get.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    private static WeatherForecast GenerateWeatherForecast(int index = 0)
        => new(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now.AddDays(index)), Random.Shared.Next(-20, 55), summaries[Random.Shared.Next(summaries.Length)]);

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
}