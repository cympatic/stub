using Cympatic.Extensions.Http;
using Cympatic.Stub.Demo.Api.Models;
using Cympatic.Stub.Demo.Api.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cympatic.Stub.Demo.Api.Services
{
    public class DemoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly DemoSettings _demoSettings;

        public DemoApiService(HttpClient httpClient, IOptions<DemoSettings> options)
        {
            _httpClient = httpClient;
            _demoSettings = options.Value;
        }

        public async Task<IEnumerable<WeatherForecast>> Get(string stubIdentifierValue)
        {
            _httpClient.DefaultRequestHeaders.Add("DemoApiIdentifier", stubIdentifierValue);

            var uri = new Uri(_demoSettings.DemoBaseUrl)
                .Append("Demo", "for", "testing")
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
