using Cympatic.Extensions.Http.Attributes;
using Cympatic.Stub.Example.Api.Models;
using Cympatic.Stub.Example.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cympatic.Stub.Example.Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    [Loggable]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ExternalApiService _service;

        public WeatherForecastController(ExternalApiService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all weather forecasts that are found via a call to the related external service
        /// </summary>
        /// <returns>A list with all weather forecasts</returns>
        /// <response code="200">A list with all weather forecasts that are returned from the related external service</response>
        /// <response code="400">Check logging for more information about the occurred error</response>         
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet()]
        public Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            return _service.Get();
        }
    }
}
