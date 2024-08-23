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
