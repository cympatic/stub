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
Creating the stub server can be done within a custom `WebApplicationFactory` [^1^], as shown custom [`WebApplicationFactory`](source/Examples/Cympatic.Stub.Example.WebApplication.IntegrationTests/Factories/ExampleWebApplicationFactory.cs) of the example testproject.

[^1^]: [Integration tests in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)

# Usage

