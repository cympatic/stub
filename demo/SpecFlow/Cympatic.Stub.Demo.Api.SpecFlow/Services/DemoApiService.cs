using Cympatic.Extensions.SpecFlow.Services;
using Cympatic.Extensions.SpecFlow.Services.Results;
using Cympatic.Stub.Demo.Api.SpecFlow.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cympatic.Stub.Demo.Api.SpecFlow.Services
{
    public class DemoApiService : ApiService
    {
        public DemoApiService(HttpClient httpClient) : base(httpClient)
        { }

        public void SetIdentifierValue(string identifierValue)
        {
            // NOTE:
            // For demo purpose only the identifierValue is added to the Request.Headers
            // In real life use a headername should be chosen that can be manipulated,
            // can contain an unique value, and is passed into all calls with the chain

            HttpClient.DefaultRequestHeaders.Add("DemoIdentifier", identifierValue);
        }

        public Task<ApiServiceResult<IEnumerable<WeatherForecast>>> GetForecastsAsync()
        {
            var uri = new Uri("weatherforecast", UriKind.Relative);

            return GetAsync<ApiServiceResult<IEnumerable<WeatherForecast>>>(uri);
        }
    }
}
