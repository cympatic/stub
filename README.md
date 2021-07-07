[![Continuous integration build](https://github.com/cympatic/stub/actions/workflows/cympatic.stub.server.ci-build.yml/badge.svg)](https://github.com/cympatic/stub/actions/workflows/cympatic.stub.server.ci-build.yml)
# Isolated system testing of microservices

The stub server is used for system testing of a microservice in an isolated environment. 

## Usage

The configuration of the microservice that's be tested in an isolated environment needs to update 
the endpoint of the external service by replacing the url of the externally used service within the configuration of the 
microservice with the url baseaddress of the stub endpoint within the stub server (for example: http[]()://localhost:32100/stub) 
plus the unique name of the system under test (for example: 
/demoapiservice).

Consider the following settings for the connectivity to the external service in *appsettings.json* of the microservice.

**appsettings.json**
```json
  "DemoApiServiceSettings": {
    "Url": "http://localhost:32100/stub/demoapiservice"
  }
```

Next to replacing the url of the externally used service to the stub server, a header 
that can used throughout the complet chain (from client call through the microservice that's 
tested to the external service) should be available and can be fillled with an unique 
identifier. This header uniquely identifies the session in which the test is executed and 
makes sure that testing can be done in parallel.

The configuration of the stub server settings in the test project consists of the following 
values:

| Name | Example | Description |
| :--- | :--- | :--- |
| `BaseAddress` | http[]()://localhost:32100 | Url to the stub server |
| `ClientName` | demoapiservice | Unique name of the system under test |
| `IdentifierHeaderName`| DemoApiIdentifier | Name of the header that's used to identify the session of the test uniquely |

Consider the following settings for the connectivity to the stub server in *appsettings.json* of the test project.

**appsettings.json**
```json
  "StubConnectivitySettings": {
    "BaseAddress": "http://localhost:32100",
    "ClientName": "DemoApiService",
    "IdentifierHeaderName": "DemoApiIdentifier"
  }
```

## Demo
The ["demo"-folder](https://github.com/cympatic/stub/tree/main/demo) contains a simple web Api project that connects to an 
external service for weather forecast models. With [SpecFlow](https://specflow.org/) features
and scenarios a BDD test is written that prepared the stub server with expected weather 
forecast models which are retrieved by the web Api and returned to the calling client when 
executing the tests. Within the test the prepared models also used as expected models.
