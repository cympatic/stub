using Cympatic.Extensions.Http;
using Cympatic.Stub.Example.Api.Models;
using Cympatic.Stub.Example.Api.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cympatic.Stub.Example.Api.Services
{
    public class ExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ExternalApiServiceSettings _externalApiServiceSettings;

        public ExternalApiService(HttpClient httpClient, IOptions<ExternalApiServiceSettings> options)
        {
            _httpClient = httpClient;
            _externalApiServiceSettings = options.Value;
        }

        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var uri = new Uri(_externalApiServiceSettings.Url)
                .Append("example", "for", "testing")
                .WithParameter("queryparam1", "test_param");

            using var response = await _httpClient.GetAsync(uri);
            {
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<WeatherForecast>>(content);
            }
        }
    }
}
