using Cympatic.Extensions.Stub.IntegrationTests.Servers;
using Cympatic.Extensions.Stub.IntegrationTests.Servers.Models;
using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Cympatic.Extensions.Stub.IntegrationTests;

public class ReceivedRequestTests : IDisposable
{
    private const int NumberOfItems = 10;

    private readonly StubServer _sut;
    private readonly TestServer _testServer;

    public ReceivedRequestTests()
    {
        _sut = new StubServer();
        _testServer = new TestServer();

        _testServer.SetBaseAddressExternalApi(_sut.BaseAddressStub);
    }

    public void Dispose()
    {
        _sut?.Dispose();
        _testServer?.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task When_TestServer_GetAll_is_called_Then_the_ResponseSetup_return_all_values_in_setup_and_the_request_is_recorded()
    {
        static IEnumerable<WeatherForecast> GetItems()
        {
            for (var i = 0; i < NumberOfItems; i++)
            {
                yield return GenerateWeatherForecast(i);
            }
        }
        var setupResponseApiService = _sut.CreateApiService<SetupResponseApiService>();
        var receivedRequestApiService = _sut.CreateApiService<ReceivedRequestApiService>();
        var testServerApiService = _testServer.CreateTestServerApiService();

        // Arrange
        var expectedResponse = GetItems().ToList();
        var responseSetup = new ResponseSetup
        {
            Path = "/external/api/weatherforecast",
            HttpMethods = [HttpMethod.Get.ToString()],
            ReturnStatusCode = HttpStatusCode.OK,
            Response = expectedResponse
        };
        var dummyResponseSetup = new ResponseSetup
        {
            Path = "/external/api/weatherforecast",
            HttpMethods = [HttpMethod.Post.ToString()],
            ReturnStatusCode = HttpStatusCode.PartialContent,
            Response = Array.Empty<WeatherForecast>()
        };
        await setupResponseApiService.AddAsync([dummyResponseSetup, responseSetup]);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, string.Empty, true)
        };

        // Act
        var actualResponse = await testServerApiService.GetAllAsync();

        // Assert
        actualResponse.Should().BeEquivalentTo(expectedResponse);

        var actualReceivedRequests = await receivedRequestApiService.FindAsync(new ReceivedRequestSearchParams("/external/api/weatherforecast", [HttpMethod.Get.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task When_TestServer_GetById_is_called_Then_the_ResponseSetup_return_all_values_in_setup_and_the_request_is_recorded()
    {
        var setupResponseApiService = _sut.CreateApiService<SetupResponseApiService>();
        var receivedRequestApiService = _sut.CreateApiService<ReceivedRequestApiService>();
        var testServerApiService = _testServer.CreateTestServerApiService();

        // Arrange
        var expectedResponse = GenerateWeatherForecast();
        var responseSetup = new ResponseSetup
        {
            Path = $"/external/api/weatherforecast/{expectedResponse.Id:N}",
            HttpMethods = [HttpMethod.Get.ToString()],
            ReturnStatusCode = HttpStatusCode.OK,
            Response = expectedResponse
        };
        var dummyResponseSetup = new ResponseSetup
        {
            Path = $"/external/api/weatherforecast/{expectedResponse.Id:N}",
            HttpMethods = [HttpMethod.Post.ToString()],
            ReturnStatusCode = HttpStatusCode.PartialContent,
            Response = GenerateWeatherForecast()
        };
        await setupResponseApiService.AddAsync([dummyResponseSetup, responseSetup]);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, string.Empty, true)
        };

        // Act
        var actualResponse = await testServerApiService.GetByIdAsync(expectedResponse.Id);

        // Assert
        actualResponse.Should().BeEquivalentTo(expectedResponse);

        var actualReceivedRequests = await receivedRequestApiService.FindAsync(new ReceivedRequestSearchParams($"/external/api/weatherforecast/{expectedResponse.Id:N}", [HttpMethod.Get.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task When_TestServer_Add_is_called_Then_the_ResponseSetup_return_all_values_in_setup_and_the_request_is_recorded()
    {
        var setupResponseApiService = _sut.CreateApiService<SetupResponseApiService>();
        var receivedRequestApiService = _sut.CreateApiService<ReceivedRequestApiService>();
        var testServerApiService = _testServer.CreateTestServerApiService();

        // Arrange
        var expectedResponse = GenerateWeatherForecast();
        var responseSetup = new ResponseSetup
        {
            Path = "/external/api/weatherforecast",
            HttpMethods = [HttpMethod.Post.ToString()],
            ReturnStatusCode = HttpStatusCode.Created,
            Response = expectedResponse,
            Location = new Uri($"/external/api/weatherforecast/{expectedResponse.Id:N}", UriKind.Relative)
        };
        var dummyResponseSetup = new ResponseSetup
        {
            Path = "/external/api/weatherforecast",
            HttpMethods = [HttpMethod.Get.ToString()],
            ReturnStatusCode = HttpStatusCode.PartialContent,
            Response = GenerateWeatherForecast()
        };
        await setupResponseApiService.AddAsync([dummyResponseSetup, responseSetup]);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, string.Empty, true)
        };

        // Act
        var actualResponse = await testServerApiService.AddAsync(expectedResponse);

        // Assert
        actualResponse.Should().BeEquivalentTo(expectedResponse);

        var actualReceivedRequests = await receivedRequestApiService.FindAsync(new ReceivedRequestSearchParams("/external/api/weatherforecast", [HttpMethod.Post.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Body)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));

        var actualRequestBody = JsonSerializer.Deserialize<WeatherForecast>(actualReceivedRequests.First().Body!);
        actualRequestBody.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task When_TestServer_Remove_is_called_Then_the_ResponseSetup_return_all_values_in_setup_and_the_request_is_recorded()
    {
        var setupResponseApiService = _sut.CreateApiService<SetupResponseApiService>();
        var receivedRequestApiService = _sut.CreateApiService<ReceivedRequestApiService>();
        var testServerApiService = _testServer.CreateTestServerApiService();

        // Arrange
        var expectedResponse = GenerateWeatherForecast();
        var responseSetup = new ResponseSetup
        {
            Path = $"/external/api/weatherforecast/{expectedResponse.Id:N}",
            HttpMethods = [HttpMethod.Delete.ToString()],
            ReturnStatusCode = HttpStatusCode.NoContent
        };
        var dummyResponseSetup = new ResponseSetup
        {
            Path = $"/external/api/weatherforecast/{expectedResponse.Id:N}",
            HttpMethods = [HttpMethod.Get.ToString()],
            ReturnStatusCode = HttpStatusCode.PartialContent
        };
        await setupResponseApiService.AddAsync([dummyResponseSetup, responseSetup]);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, string.Empty, true)
        };

        // Act
        await testServerApiService.RemoveAsync(expectedResponse);

        // Assert
        var actualReceivedRequests = await receivedRequestApiService.FindAsync(new ReceivedRequestSearchParams($"/external/api/weatherforecast/{expectedResponse.Id:N}", [HttpMethod.Delete.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    private static WeatherForecast GenerateWeatherForecast(int index = 0) => new()
    {
        Id = Guid.NewGuid(),
        Date = DateTime.Now.Date.AddDays(index),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = summaries[Random.Shared.Next(summaries.Length)]
    }; 

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
