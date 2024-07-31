using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace WebApplication1;

internal static class ApplictionBuilderExtensions
{
    public static IApplicationBuilder UseStub(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseMiddleware<StubMiddleware>();

        return app;
    }
}
