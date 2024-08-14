# Isolated testing of system components through contract testing

A C# .NET based, lightweight stub server that mimics the functionality of an external service, commonly used by microservices. 

## Key Features

- Test locally system and integration tests of system components
- Reduce the dependency on complex and/or expensive test environments
- Per-request conditional responses
- Recording requests
- Easy to use

By using [contract testing](https://microsoft.github.io/code-with-engineering-playbook/automated-testing/cdc-testing/) in integration tests for projects with dependencies on external services, the stub server can provide configurable responses for requests made to these services. Each request is recorded and can be validated as part of these integration tests.

Information about usage can be found [here](https://github.com/cympatic/stub).