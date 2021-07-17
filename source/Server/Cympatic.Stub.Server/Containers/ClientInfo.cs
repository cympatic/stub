using System;

namespace Cympatic.Stub.Server.Containers
{
    internal class ClientInfo : IDisposable, IEquatable<ClientInfo>
    {
        private bool disposedValue = false;

        public string Name { get; }

        public string IdentifierHeaderName { get; }

        public ResponseModelContainer ResponseContainer { get; private set; }

        public RequestModelContainer RequestContainer { get; private set; }

        public ClientInfo(
            string name,
            string identifierHeaderName,
            ResponseModelContainer responseContainer,
            TimeSpan responseTtl,
            RequestModelContainer requestContainer,
            TimeSpan requestTtl)
        {
            Name = name;
            IdentifierHeaderName = identifierHeaderName;

            ResponseContainer = responseContainer;
            ResponseContainer.SetTimeToLive(responseTtl);

            RequestContainer = requestContainer;
            RequestContainer.SetTimeToLive(requestTtl);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ResponseContainer.Dispose();
                    ResponseContainer = null;

                    RequestContainer.Dispose();
                    RequestContainer = null;
                }

                disposedValue = true;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClientInfo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, IdentifierHeaderName);
        }

        public bool Equals(ClientInfo other)
        {
            return other != null &&
                   Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
                   IdentifierHeaderName.Equals(other.IdentifierHeaderName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
