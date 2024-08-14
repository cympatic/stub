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

## Setup `StubServer` in a [`WebApplicationFactory`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1)
Add the initialization of the stub server in the constructor of your custom `WebApplicationFactory` and create the apiservices for setting up responses and reading received requests
``` C#
_stubServer = new StubServer();
_setupResponseApiService = _stubServer.CreateApiService<SetupResponseApiService>();
_receivedRequestApiService = _stubServer.CreateApiService<ReceivedRequestApiService>();
```

Add proxy methodes for adding responses to the stub server
``` c#
public Task<ResponseSetup> AddResponseSetupAsync(ResponseSetup responseSetup, CancellationToken cancellationToken = default)
    => _setupResponseApiService.AddAsync(responseSetup, cancellationToken);

public Task AddResponsesSetupAsync(IEnumerable<ResponseSetup> responseSetups, CancellationToken cancellationToken = default)
    => _setupResponseApiService.AddAsync(responseSetups, cancellationToken);
```

Add proxy methode for reading requests from the stub server
``` c#
public Task<IEnumerable<ReceivedRequest>> FindReceivedRequestsAsync(ReceivedRequestSearchParams searchParams, CancellationToken cancellationToken = default)
    => _receivedRequestApiService.FindAsync(searchParams, cancellationToken);
```

Add proxy methodes for removing responses and received requests from the stub server
``` c#
public Task ClearResponsesSetupAsync(CancellationToken cancellationToken = default)
    => _setupResponseApiService.RemoveAllAsync(cancellationToken);

public Task ClearReceivedRequestsAsync(CancellationToken cancellationToken = default)
    => _receivedRequestApiService.RemoveAllAsync(cancellationToken);
```

Override the `Dispose` since the stub server is a disposable object
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

Override the `CreateHost` for the `WebApplicationFactory` to configure the baseaddress of the used external service
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

Create a test class that implements a [`IClassFixture<>`](https://xunit.net/docs/shared-context#class-fixture) interface referencing the custom `WebApplicationFactory` to share object instances across the tests in the class.
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
