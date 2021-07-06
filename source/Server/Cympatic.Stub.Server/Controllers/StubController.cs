using Cympatic.Extensions.Http;
using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Extensions;
using Cympatic.Stub.Server.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Cympatic.Stub.Server.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StubController : ControllerBase
    {
        private readonly IClientContainer _clientContainer;
        private readonly ILogger _logger;

        public StubController(IClientContainer clientContainer, ILogger<StubController> logger)
        {
            _clientContainer = clientContainer;
            _logger = logger;
        }

        public async Task<IActionResult> Call()
        {
            try
            {
                var path = Request.RouteValues.TryGetValue("slug", out var slug) && slug is string slugPath
                    ? slugPath
                    : Request.Path.Value;

                var model = _clientContainer.FindResult(Request.Method, path, Request.Query);
                await RegisterRequestAsync(path, model != null);

                if (model != null)
                {
                    AddHeadersToResponse(model);

                    if (model.DelayInMilliseconds > 0)
                    {
                        await Task.Delay(model.DelayInMilliseconds);
                    }

                    return StatusCode((int)model.ReturnStatusCode, model.Result);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception occurred while retrieving a response (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);

                return BadRequest(exception.Message);
            }
            return NotFound();
        }

        private async Task RegisterRequestAsync(string path, bool responseFound)
        {
            _clientContainer.AddRequest(
                path,
                Request.Query.ToDictionary(),
                Request.Method,
                Request.Headers.ToDictionary(),
                await GetRequestBodyAsync(),
                responseFound);
        }

        private async Task<string> GetRequestBodyAsync()
        {
            Request.EnableBuffering();
            var stream = Request.Body;
            using var reader = new StreamReader(stream);
            var body = await reader.ReadToEndAsync();
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            return body;
        }

        private void AddHeadersToResponse(ResponseModel model)
        {
            Response.Headers.AddRange(model.Headers);
            if (model.ReturnStatusCode == HttpStatusCode.Created)
            {
                if (Response.Headers.ContainsKey("Location"))
                {
                    Response.Headers.Remove("Location");
                }

                Response.Headers.Add("Location", model.GetCreatedLocation(Request.Scheme, Request.Host));
            }
        }
    }
}