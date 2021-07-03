using Cympatic.Stub.Connectivity.Interfaces;

namespace Cympatic.Stub.Connectivity.Internal
{
    public class ClientStub : IClientStub
    {
        public string Name { get; }

        public string IdentifierHeaderName { get; }

        public int ResponseTtlInMinutes { get; }

        public int RequestTtlInMinutes { get; }

        public ClientStub(string name, string identifierHeaderName) : this(name, identifierHeaderName, 10, 10)
        {
        }

        public ClientStub(string name, string identifierHeaderName, int responseTtlInMinutes, int requestTtlInMinutes)
        {
            Name = name;
            IdentifierHeaderName = identifierHeaderName;
            ResponseTtlInMinutes = responseTtlInMinutes;
            RequestTtlInMinutes = requestTtlInMinutes;
        }
    }
}
