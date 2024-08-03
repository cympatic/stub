using Cympatic.Extensions.Stub.Internal.Interfaces;
using Cympatic.Extensions.Stub.Models;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Cympatic.Extensions.Stub.Internal.Middleware;

internal class StubMiddleware(RequestDelegate next)
{
    private const int MaxContentLength = 49152;

    public async Task InvokeAsync(HttpContext context, IResponseSetupCollection responseSetups, IReceivedRequestCollection receivedRequests)
    {
        if (context.Request.Path.StartsWithSegments("/stub", StringComparison.OrdinalIgnoreCase))
        {
            context.Request.EnableBuffering();
            var requestBody = await context.Request.Body.ReadAsStringAsync(true);
            if (requestBody.Length > MaxContentLength)
            {
                context.Response.StatusCode = 413;
                await context.Response.WriteAsync("Request Body Too Large");

                return;
            }
            context.Request.Body.Position = 0;

            var segments = context.Request.Path.Value?.Split("/", StringSplitOptions.RemoveEmptyEntries) ?? [];
            var uri = new Uri("/", UriKind.Relative).Append(segments[1..]);
            var path = uri.ToString();

            var responseSetup = responseSetups.Find(context.Request.Method, path, context.Request.Query).FirstOrDefault();

            receivedRequests.Add(new(path, context.Request.Method, context.Request.Query.ToDictionary(), context.Request.Headers.ToDictionary(), requestBody, responseSetup != null));

            if (responseSetup is not null)
            {
                context.Response.Headers.AddRange(responseSetup.Headers);
                if (responseSetup.ReturnStatusCode == HttpStatusCode.Created)
                {
                    if (context.Response.Headers.ContainsKey("Location"))
                    {
                        context.Response.Headers.Remove("Location");
                    }

                    context.Response.Headers.Append("Location", GetCreatedLocation(responseSetup, context.Request.Scheme, context.Request.Host));
                }

                if (responseSetup.DelayInMilliseconds > 0)
                {
                    await Task.Delay(responseSetup.DelayInMilliseconds);
                }

                context.Response.StatusCode = (int)responseSetup.ReturnStatusCode;
                if (responseSetup.Response is string responseBodyString)
                {
                    await context.Response.WriteAsync(responseBodyString);
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(responseSetup.Response);
                }
            }
        }

        await next(context);
    }

    private static string GetCreatedLocation(ResponseSetup model, string scheme, HostString host)
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
