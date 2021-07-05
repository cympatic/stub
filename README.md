[![Continuous integration build](https://github.com/cympatic/stub/actions/workflows/cympatic.stub.server.ci-build.yml/badge.svg)](https://github.com/cympatic/stub/actions/workflows/cympatic.stub.server.ci-build.yml)
# Isolated system testing of microservices

This Stub Server is used for system testing of a microservice in an isolated environment. By replacing the url of the externally used service within the configuration of the microserivce under test with the url baseaddress of the stub (for example: http://localhost:32100/stub) plus the unique name of the system under test (for example: /demoapiservice).\
Next to setting up the url to the stub, a (custom) header should be avaiable that's used throughout the complet chain (from client call through the microservice that's tested to the external service) and can be fillled with an unique identifier.

**NOTE:** This identifier uniquely identify the session in which the test is executed and makes sure that testing can be done in parallel

