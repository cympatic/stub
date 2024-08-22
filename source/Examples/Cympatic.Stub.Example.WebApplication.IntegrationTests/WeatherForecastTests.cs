using Cympatic.Extensions.Stub.Models;
using Cympatic.Stub.Example.WebApplication.IntegrationTests.Factories;
using Cympatic.Stub.Example.WebApplication.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Cympatic.Stub.Example.WebApplication.IntegrationTests;

public class WeatherForecastTests : IClassFixture<ExampleWebApplicationFactory<Program>>
{
    private const int NumberOfItems = 10;

    private readonly ExampleWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public WeatherForecastTests(ExampleWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _factory.Clear();
        _httpClient = _factory.CreateClient();
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
        actual.Should().BeEquivalentTo(expected);

        var actualReceivedRequests = await _factory.FindReceivedRequestsAsync(new ReceivedRequestSearchParams("/external/api/weatherforecast", [HttpMethod.Get.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task GetByIdWeatherForecast()
    {
        // Arrange
        var expected = GenerateWeatherForecast();
        var responseSetup = new ResponseSetup
        {
            Path = $"/external/api/weatherforecast/{expected.Id:N}",
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
        var response = await _httpClient.GetAsync($"/weatherforecast/{expected.Id:N}");

        // Assert
        var actual = await response.Content.ReadFromJsonAsync<WeatherForecast>();
        actual.Should().BeEquivalentTo(expected);

        var actualReceivedRequests = await _factory.FindReceivedRequestsAsync(new ReceivedRequestSearchParams($"/external/api/weatherforecast/{expected.Id:N}", [HttpMethod.Get.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task AddWeatherForecast()
    {
        // Arrange
        var expected = GenerateWeatherForecast();
        var responseSetup = new ResponseSetup
        {
            Path = "/external/api/weatherforecast",
            HttpMethods = [HttpMethod.Post.ToString()],
            ReturnStatusCode = HttpStatusCode.Created,
            Response = expected,
            Location = new Uri($"/external/api/weatherforecast/{expected.Id:N}", UriKind.Relative)
        };
        await _factory.AddResponseSetupAsync(responseSetup);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, JsonSerializer.Serialize(expected), true)
        };

        // Act
        var actualResponse = await _httpClient.PostAsync("/weatherforecast", new StringContent(JsonSerializer.Serialize(expected), Encoding.UTF8, "application/json"));

        // Assert
        var actual = await actualResponse.Content.ReadFromJsonAsync<WeatherForecast>();
        actual.Should().BeEquivalentTo(expected);

        var actualReceivedRequests = await _factory.FindReceivedRequestsAsync(new ReceivedRequestSearchParams("/external/api/weatherforecast", [HttpMethod.Post.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task RemoveWeatherforecast()
    {
        // Arrange
        var expected = GenerateWeatherForecast();
        var responseSetup = new ResponseSetup
        {
            Path = $"/external/api/weatherforecast/{expected.Id:N}",
            HttpMethods = [HttpMethod.Delete.ToString()],
            ReturnStatusCode = HttpStatusCode.NoContent
        };
        await _factory.AddResponseSetupAsync(responseSetup);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, string.Empty, true)
        };

        // Act
        var actualResponse = await _httpClient.DeleteAsync($"/weatherforecast/{expected.Id}");

        // Assert
        var actualReceivedRequests = await _factory.FindReceivedRequestsAsync(new ReceivedRequestSearchParams($"/external/api/weatherforecast/{expected.Id:N}", [HttpMethod.Delete.ToString()]));
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
