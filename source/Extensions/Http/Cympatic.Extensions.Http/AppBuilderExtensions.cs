using Cympatic.Extensions.Http.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Cympatic.Extensions.Http;

public static class AppBuilderExtensions
{
    // This extenstion should be place after app.UseRouting() otherwise the used endPoint in the middleware ain't working!!!
    // More information can be found at: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-5.0#routing-concepts
    public static IApplicationBuilder UseDeveloperLogging(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        var env = app.ApplicationServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;
        if (env.IsDevelopment())
        {
            return app.UseMiddleware<DeveloperLoggingMiddleware>();
        }

        return app;
    }
}
