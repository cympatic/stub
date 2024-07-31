using Cympatic.Extensions.Stub.Internal.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Cympatic.Extensions.Stub.Internal;

internal static class ApplictionBuilderExtensions
{
    public static IApplicationBuilder UseStub(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseMiddleware<StubMiddleware>();

        return app;
    }
}
