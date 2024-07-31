using Cympatic.Extensions.Stub.Internal.Collections;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Cympatic.Extensions.Stub.Internal.Middleware;

internal class StubMiddleware(RequestDelegate next)
{
    private const int MaxContentLength = 49152;

    public async Task InvokeAsync(HttpContext context, ResponseSetupCollection responseSetups, ReceivedRequestCollection receivedRequests)
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

        var responseSetup = responseSetups.Find(context.Request.Method, context.Request.Path.Value!, context.Request.Query);

        receivedRequests.Add(new(context.Request.Path.Value, context.Request.Method, context.Request.Query.ToDictionary(), context.Request.Headers.ToDictionary(), requestBody, responseSetup != null));

        if (responseSetup is not null)
        {
            context.Response.Headers.AddRange(responseSetup.Headers);
            if (responseSetup.ReturnStatusCode == HttpStatusCode.Created)
            {
                if (context.Response.Headers.ContainsKey("Location"))
                {
                    context.Response.Headers.Remove("Location");
                }

                context.Response.Headers.Append("Location", responseSetup.GetCreatedLocation(context.Request.Scheme, context.Request.Host));
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

        await next(context);
    }
}
