using Cympatic.Extensions.Http.Attributes;
using Cympatic.Stub.Connectivity.Constants;
using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cympatic.Stub.Server.Controllers;

[Produces("application/json")]
[ApiController]
[Route("[controller]")]
[Loggable]
public class SetupClientController : ControllerBase
{
    private readonly IClientContainer _clientContainer;
    private readonly ILogger _logger;

    public SetupClientController(IClientContainer clientContainer, ILogger<SetupClientController> logger)
    {
        _clientContainer = clientContainer;
        _logger = logger;
    }

    /// <summary>
    /// Add a new client
    /// </summary>
    /// <param name="client">Identifier of the client. Can be the name of the team or product that uses the Stub.Server</param>
    /// <param name="identifierHeader">Name of the header which contains the identifier value of the session when using the Stub.Server</param>
    /// <param name="responseTtlInMinutes">Time To Live (in minutes) of the registered responses for the specific client</param>
    /// <param name="requestTtlInMinutes">Time To Live (in minutes) of the registered requests for the specific client</param>
    /// <returns>The new client registered in the Stub.Server</returns>
    /// <response code="201">Returns the newly created client</response>
    /// <response code="400">Check logging for more information about the occurred error</response>         
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ClientModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("{client}/[action]")]
    public IActionResult Add(
        string client, 
        [Required] string identifierHeader = Defaults.IdentifierHeaderName,
        [Required] int responseTtlInMinutes = Defaults.ResponseTtlInMinutes,
        [Required] int requestTtlInMinutes = Defaults.RequestTtlInMinutes)
    {
        try
        {
            if (string.IsNullOrEmpty(client))
            {
                throw new ArgumentException("No clientName was given in route");
            }

            var model = _clientContainer.Add(identifierHeader, responseTtlInMinutes, requestTtlInMinutes);

            return model != default 
                ? CreatedAtAction(nameof(GetClient), new { client }, model) 
                : (IActionResult)BadRequest();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An exception occurred while adding a new client (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);

            return BadRequest(exception.Message);
        }
    }

    /// <summary>
    /// Get a client
    /// </summary>
    /// <param name="client">Identifier of the client. Can be the name of the team or product that uses the Stub.Server</param>
    /// <returns>The registered client</returns>
    /// <response code="200">Returns the requested client</response>
    /// <response code="400">Check logging for more information about the occurred error</response>         
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClientModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{client}/[action]")]
    public IActionResult GetClient(string client)
    {
        try
        {
            if (string.IsNullOrEmpty(client))
            {
                throw new ArgumentException("No clientName was given in route");
            }

            return Ok(_clientContainer.GetClient());
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An exception occurred while getting a client (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);

            return BadRequest(exception.Message);
        }
    }

    /// <summary>
    /// Get all clients
    /// </summary>
    /// <returns>A list with all available clients</returns>
    /// <response code="200">Returns all clients</response>
    /// <response code="400">Check logging for more information about the occurred error</response>         
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ClientModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("[action]")]
    public IActionResult GetClients()
    {
        try
        {
            return Ok(_clientContainer.GetClients());
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An exception occurred while getting a client (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);

            return BadRequest(exception.Message);
        }
    }


    /// <summary>
    /// Delete a client with all responses and requests 
    /// </summary>
    /// <param name="client">Identifier of the client. Can be the name of the team or product that uses the Stub.Server</param>
    /// <response code="204">Delete succeed</response>
    /// <response code="400">Check logging for more information about the occurred error</response>         
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{client}/[action]")]
    public IActionResult Remove(string client)
    {
        try
        {
            if (string.IsNullOrEmpty(client))
            {
                throw new ArgumentException("No clientName was given in route");
            }

            _clientContainer.Remove();

            return NoContent();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An exception occurred while deleting client (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);

            return BadRequest(exception.Message);
        }
    }
}