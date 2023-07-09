using Cympatic.Extensions.Http.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cympatic.Extensions.Http.Middleware;

public class DeveloperLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public DeveloperLoggingMiddleware(RequestDelegate next, ILogger<DeveloperLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            var endpoint = httpContext.GetEndpoint();

            if (endpoint?.Metadata.OfType<LoggableAttribute>().Any() ?? false)
            {
                var request = httpContext.Request;
                var requestTime = DateTime.Now;
                var requestBodyContent = await ReadRequestBody(request);

                var stopWatch = Stopwatch.StartNew();
                var originalBodyStream = httpContext.Response.Body;
                try
                {
                    var response = httpContext.Response;

                    using var responseBody = new MemoryStream();
                    response.Body = responseBody;

                    await _next(httpContext);

                    stopWatch.Stop();

                    var responseBodyContent = await ReadResponseBody(response);
                    await responseBody.CopyToAsync(originalBodyStream);

                    SafeLogRequest(requestTime,
                        request.Method,
                        request.Path,
                        request.QueryString.ToString(),
                        requestBodyContent);

                    SafeLogResponse(stopWatch.ElapsedMilliseconds, response.StatusCode, responseBodyContent);
                }
                finally
                {
                    httpContext.Response.Body = originalBodyStream;
                }
            }
            else
            {
                await _next(httpContext);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(DeveloperLoggingMiddleware)}");
            throw;
        }
    }

    private static async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(new Memory<byte>(buffer));
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body.Seek(0, SeekOrigin.Begin);

        return bodyAsText;
    }

    private static async Task<string> ReadResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return bodyAsText;
    }

    private void SafeLogRequest(DateTime requestTime, string method, PathString path, string queryString, string requestBody)
    {
        _logger.LogDebug("RequestTime = {requestTime}", requestTime);
        _logger.LogDebug("Method = {method}", method);
        _logger.LogDebug("Path = {path}", path);
        _logger.LogDebug("QueryString = {queryString}", queryString);
        _logger.LogDebug("RequestBody = {requestBody}", requestBody);
    }

    private void SafeLogResponse(long responseMillis, int statusCode, string responseBody)
    {
        _logger.LogDebug("ResponseMillis = {responseMillis}", responseMillis);
        _logger.LogDebug("StatusCode = {statusCode}", statusCode);
        _logger.LogDebug("ResponseBody = {responseBody}", responseBody);
    }
}
