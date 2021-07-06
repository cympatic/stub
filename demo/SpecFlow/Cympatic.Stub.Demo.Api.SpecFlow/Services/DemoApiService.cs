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

        public async Task<IEnumerable<WeatherForecast>> GetForecasts(string stubIdentifierValue)
        {
            _httpClient.DefaultRequestHeaders.Add("DemoIdentifier", stubIdentifierValue);

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
