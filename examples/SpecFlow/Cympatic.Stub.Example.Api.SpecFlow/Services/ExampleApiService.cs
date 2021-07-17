using Cympatic.Extensions.SpecFlow.Services;
using Cympatic.Extensions.SpecFlow.Services.Results;
using Cympatic.Stub.Example.Api.SpecFlow.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cympatic.Stub.Example.Api.SpecFlow.Services
{
    public class ExampleApiService : ApiService
    {
        public ExampleApiService(HttpClient httpClient) : base(httpClient)
        { }

        public void SetIdentifierValue(string identifierValue)
        {
            // NOTE:
            // For example purpose only the identifierValue is added to the Request.Headers
            // In real life use a headername should be chosen that can be manipulated,
            // can contain an unique value, and is passed into all calls with the chain

            HttpClient.DefaultRequestHeaders.Add("ExampleIdentifier", identifierValue);
        }

        public Task<ApiServiceResult<IEnumerable<WeatherForecast>>> GetForecastsAsync()
        {
            var uri = new Uri("weatherforecast", UriKind.Relative);

            return GetAsync<ApiServiceResult<IEnumerable<WeatherForecast>>>(uri);
        }
    }
}
