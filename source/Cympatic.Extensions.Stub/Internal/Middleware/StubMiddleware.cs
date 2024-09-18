using Cympatic.Extensions.Stub.Internal;
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

            var path = GetPath(context.Request.Path);

            var responseSetup = responseSetups.Find(context.Request.Method, path, context.Request.Query).FirstOrDefault();

            receivedRequests.Add(new(path, context.Request.Method, context.Request.Query.ToDictionary(), context.Request.Headers.ToDictionary(), requestBody, responseSetup != null));

            if (responseSetup is not null)
            {
                SetResponseHeaders(context.Response.Headers, responseSetup, () => GetCreatedLocation(responseSetup, context.Request.Scheme, context.Request.Host));

                if (responseSetup.DelayInMilliseconds > 0)
                {
                    await Task.Delay(responseSetup.DelayInMilliseconds);
                }

                context.Response.StatusCode = (int)responseSetup.ReturnStatusCode;
                if (responseSetup.Response is string responseBodyString)
                {
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(responseBodyString);
                }
                else if (responseSetup.Response is not null)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(responseSetup.Response);
                }

                return;
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "text/plain; charset=UTF-8";
            await context.Response.WriteAsync($"No ResponseSetup is found for path: {path}");

            return;
        }

        await next(context);
    }

    private static string GetPath(PathString path)
    {
        var segments = path.Value?.Split("/", StringSplitOptions.RemoveEmptyEntries) ?? [];
        var uri = new Uri("/", UriKind.Relative).Append(segments[1..]);
        return uri.ToString();
    }

    private static void SetResponseHeaders(IHeaderDictionary headers, ResponseSetup responseSetup, Func<string> createdLocation)
    {
        headers.AddRange(responseSetup.Headers);
        if (responseSetup.ReturnStatusCode == HttpStatusCode.Created)
        {
            if (headers.ContainsKey("Location"))
            {
                headers.Remove("Location");
            }

            headers.Append("Location", createdLocation?.Invoke() ?? string.Empty);
        }
    }

    private static string GetCreatedLocation(ResponseSetup responseSetup, string scheme, HostString host)
    {
        if (host.HasValue && responseSetup.Location != null && !responseSetup.Location.IsAbsoluteUri)
        {
            var uriBuilder = host.Port.HasValue
                ? new UriBuilder(scheme, host.Host, host.Port.Value)
                : new UriBuilder(scheme, host.Host);

            return uriBuilder.Uri.Append(responseSetup.Location.ToString()).ToString();
        }

        return responseSetup.Location?.ToString() ?? string.Empty;
    }
}
