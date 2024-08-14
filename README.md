[![Continuous integration build](https://github.com/cympatic/stub/actions/workflows/ci.yml/badge.svg)](https://github.com/cympatic/stub)
[![NuGet](https://img.shields.io/nuget/v/Cympatic.Extensions.Stub)](https://www.nuget.org/packages/Cympatic.Extensions.Stub)
# Isolated system testing of microservices

A C# .NET based, lightweight stub server that mimics the functionality of an external service, commonly used by microservices.

## Key Features

- Isolated system and integration tests of microservices
- Per-request conditional responses
- Recording requests

In integration tests of projects that have dependencies to external services, the stub server can provide configurable responses for the requests made to an external service. Each request is recording and can be validated as part of the integration tests.

> [!NOTE]
> In discussions of integration tests, the tested project is frequently called the System Under Test, or "SUT" for short. 

The stub server creates a web host for the external service to handle the requests and responses for the external service made by the SUT. Creating the stub server can be done within a custom `WebApplicationFactory` [^1^] that might be available in the testproject for integration testing the SUT. An example of a custom [`WebApplicationFactory`](source/Examples/Cympatic.Stub.Example.WebApplication.IntegrationTests/Factories/ExampleWebApplicationFactory.cs) can be found in the [example testproject](source/Examples/Cympatic.Stub.Example.WebApplication.IntegrationTests).

[^1^]: [Integration tests in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)

# Usage

## Setup `StubServer` in a [WebApplicationFactory](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1)
Add the initialization of the stub server in the constructor of your custom `WebApplicationFactory` and create the apiservices for setting up responses and reading received requests
``` C#
_stubServer = new StubServer();
_setupResponseApiService = _stubServer.CreateApiService<SetupResponseApiService>();
_receivedRequestApiService = _stubServer.CreateApiService<ReceivedRequestApiService>();
```

Add proxy methodes for adding responses to the `StubServer`
``` c#
public Task<ResponseSetup> AddResponseSetupAsync(ResponseSetup responseSetup, CancellationToken cancellationToken = default)
    => _setupResponseApiService.AddAsync(responseSetup, cancellationToken);

public Task AddResponsesSetupAsync(IEnumerable<ResponseSetup> responseSetups, CancellationToken cancellationToken = default)
    => _setupResponseApiService.AddAsync(responseSetups, cancellationToken);
```

Add proxy methode for reading requests from the `StubServer`
``` c#
public Task<IEnumerable<ReceivedRequest>> FindReceivedRequestsAsync(ReceivedRequestSearchParams searchParams, CancellationToken cancellationToken = default)
    => _receivedRequestApiService.FindAsync(searchParams, cancellationToken);
```

Add proxy methodes for removing responses and received requests from the `StubServer`
``` c#
public Task ClearResponsesSetupAsync(CancellationToken cancellationToken = default)
    => _setupResponseApiService.RemoveAllAsync(cancellationToken);

public Task ClearReceivedRequestsAsync(CancellationToken cancellationToken = default)
    => _receivedRequestApiService.RemoveAllAsync(cancellationToken);
```

Override the `Dispose` since the `StubServer` is a disposable object
``` C#
protected override void Dispose(bool disposing)
{
    base.Dispose(disposing);

    if (disposing)
    {
        _stubServer.Dispose();
    }
}
```

Override the `CreateHost` of the [WebApplicationFactory](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1) to configure the baseaddress of the used external service
``` C#
protected override IHost CreateHost(IHostBuilder builder)
{
    builder.ConfigureServices((context, services) =>
    {
        context.Configuration["ExternalApi"] = _stubServer.BaseAddressStub.ToString();
    });

    return base.CreateHost(builder);
}
```

## Use in unit test

Create a test class that implements a [IClassFixture<>](https://xunit.net/docs/shared-context#class-fixture) interface referencing the custom `WebApplicationFactory` to share object instances across the tests in the class.
``` C#
public class WeatherForecastTests : IClassFixture<ExampleWebApplicationFactory<Program>>
```

In the constructor of the test class use the factory to create the `HttpClient` and clear the `ResponseSetup` and `ReceivedRequest` of the `StubServer`
``` C#
public WeatherForecastTests(ExampleWebApplicationFactory<Program> factory)
{
    _factory = factory;
    _httpClient = _factory.CreateClient();

    _factory.ClearResponsesSetupAsync();
    _factory.ClearReceivedRequestsAsync();
}
```

A typical test uses the factory to setup the response and process the request through the `HttpClient`. The request to the external service can be validated.
``` C#
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
```
- Prepare the `ResponseSetup`:
    - Set `Path` and `HttpMethods` to a partial path and HttpMethod of the expected request used by the external service.
    - Set `ReturnStatusCode` to the desired [HttpStatusCode](https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode).
    - Set `Response` to the desired reponse of the external service.
- Add the `ResponseSetup` to the `StubServer` with the method `AddResponseSetupAsync`

> [!TIP]
> With the method `AddResponsesSetupAsync` one can add multiple `ResponseSetup` in 1 call!

- Process the request to the SUT using the `HttpClient`
- Verify the response from the SUT
- Verify the request made to the external service
    - Use the method `FindReceivedRequestsAsync` to locate the request made to the external service. A find of a `ReceivedRequest` is on a combination of `Path`, `HttpMethod`, and `Query`.
> [!IMPORTANT]
> `ReceivedRequest` can only be found when there is a matching `ResponseSetup` 
