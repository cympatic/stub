using Cympatic.Stub.Abstractions.Models;
using Cympatic.Stub.Server.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Cympatic.Stub.Server.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class VerifyRequestController : ControllerBase
    {
        private readonly IClientContainer _clientContainer;
        private readonly ILogger _logger;

        public VerifyRequestController(IClientContainer clientContainer, ILogger<VerifyRequestController> logger)
        {
            _clientContainer = clientContainer;
            _logger = logger;
        }

        /// <summary>
        /// Get all registered requests that are posted for a client and session (via identifierValue)
        /// </summary>
        /// <param name="client">Identifier of the client. Can be the name of the team or product that uses the Stub.Server</param>
        /// <returns>A list with all requests the given client and session</returns>
        /// <response code="200">A list with all requests that are posted for the given client and session</response>
        /// <response code="400">Check logging for more information about the occurred error</response>         
        [ProducesResponseType(typeof(IEnumerable<RequestModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{client}/[action]")]
        public IActionResult GetAll(string client)
        {
            try
            {
                if (string.IsNullOrEmpty(client))
                {
                    throw new ArgumentException("No clientName was given in route");
                }

                var models = _clientContainer.GetRequests();

                return Ok(models);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception occurred while retrieving excepted received requests (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);

                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Get registered requests that are posted for a client and session (via identifierValue) that matches the search criteria
        /// </summary>
        /// <param name="client">Identifier of the client. Can be the name of the team or product that uses the Stub.Server</param>
        /// <param name="searchModel"></param>
        /// <returns>A list with requests the given client and session that matches the search criteria</returns>
        /// <response code="200">A list with requests that are posted and matches the search criteria for the given client and session</response>
        /// <response code="400">Check logging for more information about the occurred error</response>         
        [ProducesResponseType(typeof(IEnumerable<RequestModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{client}/[action]")]
        public IActionResult Search(string client, [FromQuery] RequestSearchModel searchModel)
        {
            try
            {
                if (string.IsNullOrEmpty(client))
                {
                    throw new ArgumentException("No clientName was given in route");
                }

                var models = _clientContainer.SearchRequests(searchModel);

                return Ok(models);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception occurred while retrieving excepted received requests (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);

                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Delete all registered requests for a client and session (via identifierValue)
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

                _clientContainer.RemoveRequests();

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception occurred while clearing all received requests (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);

                return BadRequest(exception.Message);
            }
        }
    }
}
