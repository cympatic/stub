using System;

namespace Cympatic.Stub.Connectivity.Settings;

public class StubConnectivitySettings
{
    public string Name { get; set; } = Guid.NewGuid().ToString("N");

    public string BaseAddress { get; set; }

    public string IdentificationHeaderName { get; set; }

    public bool UseIdentificationHeader
        => !string.IsNullOrEmpty(IdentificationHeaderName);
}
