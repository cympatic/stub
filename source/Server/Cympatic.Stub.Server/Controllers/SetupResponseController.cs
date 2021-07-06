using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cympatic.Stub.Server.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class SetupResponseController : ControllerBase
    {
        private readonly IClientContainer _clientContainer;
        private readonly ILogger _logger;

        public SetupResponseController(IClientContainer clientContainer, ILogger<SetupResponseController> logger)
        {
            _clientContainer = clientContainer;
            _logger = logger;
        }

        /// <summary>
        /// Get all registered responses for a client and session (via identifierValue)
        /// </summary>
        /// <param name="client">Identifier of the client. Can be the name of the team or product that uses the Stub.Server</param>
        /// <returns>A list with all responses the given client and session</returns>
        /// <response code="200">A list with all responses for the given client and session</response>
        /// <response code="400">Check logging for more information about the occurred error</response>         
        [ProducesResponseType(typeof(IEnumerable<ResponseModel>), StatusCodes.Status200OK)]
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

                var models = _clientContainer.GetResponses();

                return Ok(models);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception occurred while retrieving models (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Add new responses or update registered responses for a client and session (via identifierValue)
        /// </summary>
        /// <param name="client">Identifier of the client. Can be the name of the team or product that uses the Stub.Server</param>
        /// <param name="models">List with new responses or registered responses</param>
        /// <returns>A list with all responses that are added or updated for the given client and session</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /AddOrUpdate
        ///     [
        ///       {
        ///         "httpMethods": [
        ///           "GET",
        ///           "POST"
        ///         ],
        ///         "returnStatusCode": 201,
        ///         "location": null,
        ///         "result": {
        ///           "customerId": 5,
        ///           "customerName": "Pepsi"
        ///         },
        ///         "path": "segment1/segment2",
        ///         "query": {
        ///           "queryparam1": "1"
        ///         }
        ///       },
        ///       {
        ///         "httpMethods": [],
        ///         "returnStatusCode": 201,
        ///         "location": "/created/at/location/2",
        ///         "result": {
        ///           "customerId": 5,
        ///           "customerName": "Pepsi"
        ///         },
        ///         "path": "segment1/{*wildcard}/segment3",
        ///         "query": {
        ///           "queryparam1": "1",
        ///           "queryparam2": "{*wildcard}"
        ///         },
        ///         "headers": {
        ///           "Accept": [
        ///             "application/json"
        ///           ]
        ///         }
        ///       }
        ///     ]
        ///
        /// </remarks>
        /// <response code="200">A list with all added or updated responses the given client and session</response>
        /// <response code="400">Check logging for more information about the occurred error</response>         
        [ProducesResponseType(typeof(IEnumerable<ResponseModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{client}/[action]")]
        public IActionResult AddOrUpdate(string client, [Required] IEnumerable<ResponseModel> models)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (string.IsNullOrEmpty(client))
                {
                    throw new ArgumentException("No clientName was given in route");
                }

                _clientContainer.AddOrUpdateResponses(models);

                return CreatedAtAction(nameof(GetAll), new { client }, models);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception occurred while adding or updating models (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);

                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Delete all registered responses for a client and session (via identifierValue)
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

                _clientContainer.RemoveResponses();

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception occurred while retrieving models (routevalues: '{routeValues}', queryParams: '{queryParams}')", Request.RouteValues, Request.QueryString);

                return BadRequest(exception.Message);
            }
        }
    }
}