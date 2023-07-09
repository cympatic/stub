using Cympatic.Stub.Connectivity.Constants;
using Cympatic.Stub.Connectivity.Interfaces;

namespace Cympatic.Stub.Connectivity.Internal;

public class ClientStub : IClientStub
{
    public string Name { get; }

    public string IdentifierHeaderName { get; }

    public int ResponseTtlInMinutes { get; }

    public int RequestTtlInMinutes { get; }

    public ClientStub(string name)
        : this(name, Defaults.IdentifierHeaderName, Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes)
    { }

    public ClientStub(string name, string identifierHeaderName) 
        : this(name, identifierHeaderName, Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes)
    { }

    public ClientStub(string name, string identifierHeaderName, int responseTtlInMinutes, int requestTtlInMinutes)
    {
        Name = name;
        IdentifierHeaderName = !string.IsNullOrEmpty(identifierHeaderName)
            ? identifierHeaderName
            : Defaults.IdentifierHeaderName;
        ResponseTtlInMinutes = responseTtlInMinutes;
        RequestTtlInMinutes = requestTtlInMinutes;
    }
}
