using Cympatic.Extensions.Stub.Models;
using Microsoft.AspNetCore.Http;

namespace Cympatic.Extensions.Stub.Internal;

internal static class ResponseSetupExtensions
{
    public static string GetCreatedLocation(this ResponseSetup model, string scheme, HostString host)
    {
        if (host.HasValue && model.Location != null && !model.Location.IsAbsoluteUri)
        {
            var uriBuilder = host.Port.HasValue
                ? new UriBuilder(scheme, host.Host, host.Port.Value)
                : new UriBuilder(scheme, host.Host);

            return uriBuilder.Uri.Append(model.Location.ToString()).ToString();
        }

        return model.Location?.ToString() ?? string.Empty;
    }

}
