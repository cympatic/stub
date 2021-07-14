using Cympatic.Stub.Demo.Api.SpecFlow.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cympatic.Stub.Demo.Api.SpecFlow.Services
{
    public class DemoApiService
    {
        private readonly HttpClient _httpClient;

        public DemoApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetIdentifierValue(string identifierValue)
        {
            // NOTE:
            // For demo purpose only the identifierValue is added to the Request.Headers
            // In real life use a headername should be chosen that can be manipulated,
            // can contain an unique value, and is passed into all calls with the chain

            _httpClient.DefaultRequestHeaders.Add("DemoIdentifier", identifierValue);
        }

        public async Task<IEnumerable<WeatherForecast>> GetForecasts()
        {
            var uri = new Uri("weatherforecast", UriKind.Relative);

            using var response = await _httpClient.GetAsync(uri);
            {
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var models = JsonSerializer.Deserialize<List<WeatherForecast>>(content);
                return models;
            }
        }
    }
}
