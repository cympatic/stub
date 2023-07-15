namespace Cympatic.Stub.Connectivity.Interfaces;

public interface IClientStub
{
    string Name { get; }

    string IdentifierHeaderName { get; }

    int ResponseTtlInMinutes { get; }

    int RequestTtlInMinutes { get; }
}