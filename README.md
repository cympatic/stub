[![Continuous integration build](https://github.com/cympatic/stub/actions/workflows/cympatic.stub.server.ci-build.yml/badge.svg)](https://github.com/cympatic/stub/actions/workflows/cympatic.stub.server.ci-build.yml)
# Isolated system testing of microservices

A C# .NET based, lightweight stub server to mimics functionality of an externally service used by microservices.

## Key Features

- Isolated system and integration tests of microservices
- Per-request conditional responses
- Recording requests
- Can be used as (local) service or in CI/CD scenarios
- Supporting libraries for implementing in SpecFlow

# UNDER CONSTRUCTION !!!
Currently the stub is undergoing a complete redesign and rebuild.

NOTE: Check the branch [refactor/server](refactor/server) for progress

---

## Usage

The configuration of the microservice that's be tested in an isolated environment needs to update 
the endpoint of the external service by replacing the url of the externally used service within the configuration of the 
microservice with the url baseaddress of the stub endpoint within the stub server (for example: http[]()://localhost:32100/stub) 
plus an unique name of the system under test (for example: exampleapiservice).

Consider the following settings for the connectivity to the external service in *appsettings.json* of the microservice.

**appsettings.json**
```json
  "ExternalApiServiceSettings": {
    "Url": "http://localhost:32100/stub/exampleapiservice"
  }
```

Next to replacing the url of the externally used service, a header that is used throughout the complet chain 
(from a client call throughout the microservice that's tested into the call to the external service) should be available 
and can be filled with an unique identifier. This header uniquely identifies the session in which the test is executed and 
makes sure that testing can be done in parallel. The name of header can be configured in the *appsettings.json* of the
test project.

The connectivity to the stub server is configured in the `StubConnectivitySettings` section in the *appsettings.json* of the test 
project and consists of the following members:

| Name | Description |Example | 
| :--- | :--- | :--- |
| `BaseAddress` | Url to the stub server | http[]()://localhost:32100 | 
| `ClientName` | Unique name of the system under test | ExampleApiService | 
| `IdentifierHeaderName`| Name of the header that's used to identify the session of the test uniquely (**Default value:** StubIdentifierValue) | ExampleIdentifier | 

Consider the following settings for the connectivity to the stub server in *appsettings.json* of the test project.

**appsettings.json**
```json
  "StubConnectivitySettings": {
    "BaseAddress": "http://localhost:32100",
    "ClientName": "ExampleApiService",
    "IdentifierHeaderName": "ExampleIdentifier"
  }
```

## Examples
The ["examples"-folder](examples) contains a simple web Api project that connects to an 
external service for weather forecast models. With [SpecFlow](https://specflow.org/) features
and scenarios a BDD test is written that prepared the stub server with expected weather 
forecast models which are retrieved by the web Api and returned to the calling client when 
executing the tests. Within the test the prepared models also used as expected models.
