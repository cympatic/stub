using Cympatic.Stub.Abstractions.Constants;
using Cympatic.Stub.Connectivity.Interfaces;
using System;

namespace Cympatic.Stub.Connectivity.Internal
{
    internal class DefaultClientStub : IClientStub
    {
        private static readonly string _defaultName = Guid.NewGuid().ToString("N");

        public string Name => _defaultName;

        public string IdentifierHeaderName => Defaults.IdentifierHeaderName;

        public int ResponseTtlInMinutes => Defaults.ResponseTtlInMinutes;

        public int RequestTtlInMinutes => Defaults.RequestTtlInMinutes;
    }
}
