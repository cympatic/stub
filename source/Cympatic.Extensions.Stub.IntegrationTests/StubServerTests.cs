using Cympatic.Extensions.Stub.IntegrationTests.Fixtures;
using Cympatic.Extensions.Stub.IntegrationTests.Servers;
using Cympatic.Extensions.Stub.IntegrationTests.Servers.Models;
using Cympatic.Extensions.Stub.Models;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Cympatic.Extensions.Stub.IntegrationTests;

public class StubServerTests : IClassFixture<StubServerFixture>
{
    private const int NumberOfItems = 10;

    private readonly StubServerFixture _fixture;
    private readonly StubServer _sut;
    private readonly TestServer _testServer;

    public StubServerTests(StubServerFixture fixture)
    {
        _fixture = fixture;
        _fixture.Clear();

        _sut = _fixture.StubServer;
        _testServer = _fixture.TestServer;
    }

    [Fact]
    public async Task When_TestServer_GetAll_is_called_Then_the_ResponseSetup_return_all_values_in_setup_and_the_request_is_recorded()
    {
        IEnumerable<WeatherForecast> GetItems()
        {
            for (var i = 0; i < NumberOfItems; i++)
            {
                yield return _fixture.GenerateWeatherForecast(i);
            }
        }
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
        await _sut.AddResponsesSetupAsync([dummyResponseSetup, responseSetup]);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, string.Empty, true)
        };

        // Act
        var actualResponse = await testServerApiService.GetAllAsync();

        // Assert
        actualResponse.Should().BeEquivalentTo(expectedResponse);

        var actualReceivedRequests = await _sut.FindReceivedRequestsAsync(new ReceivedRequestSearchParams("/external/api/weatherforecast", [HttpMethod.Get.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task When_TestServer_GetById_is_called_Then_the_ResponseSetup_return_all_values_in_setup_and_the_request_is_recorded()
    {
        var testServerApiService = _testServer.CreateTestServerApiService();

        // Arrange
        var expectedResponse = _fixture.GenerateWeatherForecast();
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
            Response = _fixture.GenerateWeatherForecast()
        };
        await _sut.AddResponsesSetupAsync([dummyResponseSetup, responseSetup]);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, string.Empty, true)
        };

        // Act
        var actualResponse = await testServerApiService.GetByIdAsync(expectedResponse.Id);

        // Assert
        actualResponse.Should().BeEquivalentTo(expectedResponse);

        var actualReceivedRequests = await _sut.FindReceivedRequestsAsync(new ReceivedRequestSearchParams($"/external/api/weatherforecast/{expectedResponse.Id:N}", [HttpMethod.Get.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task When_TestServer_Add_is_called_Then_the_ResponseSetup_return_all_values_in_setup_and_the_request_is_recorded()
    {
        var testServerApiService = _testServer.CreateTestServerApiService();

        // Arrange
        var expectedResponse = _fixture.GenerateWeatherForecast();
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
            Response = _fixture.GenerateWeatherForecast()
        };
        await _sut.AddResponsesSetupAsync([dummyResponseSetup, responseSetup]);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, string.Empty, true)
        };

        // Act
        var actualResponse = await testServerApiService.AddAsync(expectedResponse);

        // Assert
        actualResponse.Should().BeEquivalentTo(expectedResponse);

        var actualReceivedRequests = await _sut.FindReceivedRequestsAsync(new ReceivedRequestSearchParams("/external/api/weatherforecast", [HttpMethod.Post.ToString()]));
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
        var testServerApiService = _testServer.CreateTestServerApiService();

        // Arrange
        var expectedResponse = _fixture.GenerateWeatherForecast();
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
        await _sut.AddResponsesSetupAsync([dummyResponseSetup, responseSetup]);

        var expectedReceivedRequests = new List<ReceivedRequest>
        {
            new(responseSetup.Path, responseSetup.HttpMethods[0], responseSetup.Query, responseSetup.Headers, string.Empty, true)
        };

        // Act
        await testServerApiService.RemoveAsync(expectedResponse);

        // Assert
        var actualReceivedRequests = await _sut.FindReceivedRequestsAsync(new ReceivedRequestSearchParams($"/external/api/weatherforecast/{expectedResponse.Id:N}", [HttpMethod.Delete.ToString()]));
        actualReceivedRequests.Should().BeEquivalentTo(expectedReceivedRequests, options => options
            .Excluding(_ => _.Headers)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }
}
