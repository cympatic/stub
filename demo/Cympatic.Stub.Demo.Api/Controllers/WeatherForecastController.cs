using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cympatic.Stub.Demo.Api.Models;
using Cympatic.Stub.Demo.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cympatic.Stub.Demo.Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly DemoApiService _service;

        public WeatherForecastController(DemoApiService service)
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
            var stubIdentifierValue = Request.Headers["DemoIdentifier"];

            return _service.Get(stubIdentifierValue.FirstOrDefault());
        }
    }
}
