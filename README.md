[![Continuous integration build](https://github.com/cympatic/stub/actions/workflows/ci.yml/badge.svg)](https://github.com/cympatic/stub)
[![NuGet](https://img.shields.io/nuget/v/Cympatic.Extensions.Stub)](https://www.nuget.org/packages/Cympatic.Extensions.Stub)
# Isolated testing of system components through contract testing

A C# .NET based, lightweight stub server that mimics the functionality of an external service, commonly used by microservices. 

## Key Features

- Test locally system and integration tests of system components
- Reduce the dependency on complex and/or expensive test environments
- Per-request conditional responses
- Recording requests
- Easy to use

By using contract testing[^1^] in integration tests for projects with dependencies on external services, the stub server can provide configurable responses for requests made to these services. Each request is recorded and can be validated as part of these integration tests.
[^1^]: [Consumer-driven Contract Testing (CDC)](https://microsoft.github.io/code-with-engineering-playbook/automated-testing/cdc-testing/)

> [!NOTE]
> In discussions of integration tests, the tested project is frequently called "SUT", the System Under Test in short. 

The stub server creates a in-memory web host for the external service to handle the requests and responses for the external service made by the SUT. Creating the stub server can be done within a custom `WebApplicationFactory` [^2^] that might be available in the testproject for integration testing the SUT. An example of a custom [`WebApplicationFactory`](source/Examples/Cympatic.Stub.Example.WebApplication.IntegrationTests/Factories/ExampleWebApplicationFactory.cs) can be found in the [example testproject](source/Examples/Cympatic.Stub.Example.WebApplication.IntegrationTests).

[^2^]: [Integration tests in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)

# Usage

## Setup `StubServer` in a custom [WebApplicationFactory](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1)
Add the initialization of the stub server in the constructor of your custom `WebApplicationFactory`.
``` C#
_stubServer = new StubServer();
```

Add proxy methodes for adding responses to the `StubServer`.
``` c#
public Task<ResponseSetup> AddResponseSetupAsync(ResponseSetup responseSetup, CancellationToken cancellationToken = default)
    => _stubServer.AddResponseSetupAsync(responseSetup, cancellationToken);

public Task AddResponsesSetupAsync(IEnumerable<ResponseSetup> responseSetups, CancellationToken cancellationToken = default)
    => _stubServer.AddResponsesSetupAsync(responseSetups, cancellationToken);
```

Add proxy methode for reading requests from the `StubServer`.
``` c#
public Task<IEnumerable<ReceivedRequest>> FindReceivedRequestsAsync(ReceivedRequestSearchParams searchParams, CancellationToken cancellationToken = default)
    => _stubServer.FindReceivedRequestsAsync(searchParams, cancellationToken);
```

Add proxy methodes for removing responses and received requests from the `StubServer`.
``` c#
public Task ClearResponsesSetupAsync(CancellationToken cancellationToken = default)
    => _stubServer.ClearResponsesSetupAsync(cancellationToken);

public Task ClearReceivedRequestsAsync(CancellationToken cancellationToken = default)
    => _stubServer.ClearReceivedRequestsAsync(cancellationToken);
```

Override the `Dispose` since the `StubServer` is a disposable object.
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

Override the `CreateHost` of the [WebApplicationFactory](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1) to configure the base address of the used external service.
``` C#
protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.ConfigureServices((context, services) =>
    {
        context.Configuration["ExternalApi"] = _stubServer.BaseAddressStub.ToString();
    });

    base.ConfigureWebHost(builder);
}
```

## Use in unit test

Create a test class that implements a [IClassFixture<>](https://xunit.net/docs/shared-context#class-fixture) interface referencing the custom `WebApplicationFactory` to share object instances across the tests in the class.
``` C#
public class WeatherForecastTests : IClassFixture<ExampleWebApplicationFactory<Program>>
```

In the constructor of the test class use the factory to create the `HttpClient` and clear the `ResponseSetup` and `ReceivedRequest` data from the `StubServer`
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
> Multiple `ResponseSetup` can be added in 1 call with the method `AddResponsesSetupAsync`!

- Process the request to the SUT using the `HttpClient`
- Verify the response from the SUT
- Verify the request made to the external service
    - Use the method `FindReceivedRequestsAsync` to locate the request made to the external service. The request can be found on a combination of `Path`, `HttpMethod`, and `Query`.

> [!IMPORTANT]
> `ReceivedRequest` can only be found when there is a matching `ResponseSetup` 

---

## Release notes

1.0.3
- Change the default for the usage of the development certificate in the `StubServer` from `true` to `false`
- Changed constructor of `StubServer` so that users can chose a certificate used by the `StubServer` and how to handle this certificate in the `HttpClient`
- Add methods for add and remove `ResponseSetup` items to the `StubServer`
- Add methods for find and remove `ReceivedRequest` to the `StubServer`
- [Deprecated] methods in `StubServer`:
  - `CreateApiService<TApiService>`
  - `CreateApiService(type type>)`
