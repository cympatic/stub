[![Continuous integration build](https://github.com/cympatic/stub/actions/workflows/ci.yml/badge.svg)](https://github.com/cympatic/stub)
[![NuGet](https://img.shields.io/nuget/v/Cympatic.Extensions.Stub)](https://www.nuget.org/packages/Cympatic.Extensions.Stub)
# Isolated system testing of microservices

A C# .NET based, lightweight stub server that mimics the functionality of an external service, commonly used by microservices.

## Key Features

- Isolated system and integration tests of microservices
- Per-request conditional responses
- Recording requests

In integration tests of projects that have dependencies to external services, the stub server can provide configurable responses for the requests made to an external service. 
Each request is recording and can be validated as part of the integration tests.

> [!NOTE]
> In discussions of integration tests, the tested project is frequently called the System Under Test, or "SUT" for short. 

The stub server creates a web host for the external service to handle the requests and responses for the external service made by the SUT. 
Creating the stub server can be done within a custom `WebApplicationFactory` [^1^] that might be available in the testproject for integration testing the SUT. 
An example of a custom [`WebApplicationFactory`](source/Examples/Cympatic.Stub.Example.WebApplication.IntegrationTests/Factories/ExampleWebApplicationFactory.cs) can be found in the [example testproject](source/Examples/Cympatic.Stub.Example.WebApplication.IntegrationTests).

[^1^]: [Integration tests in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)

# Usage

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


