﻿using Cympatic.Extensions.Http;
using Cympatic.Extensions.Http.Services;
using Cympatic.Extensions.Http.Services.Results;
using Cympatic.Stub.Example.Api.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Stub.Example.Api.Services;

public class ExternalApiService : ApiService
{
    public ExternalApiService(HttpClient httpClient) : base(httpClient)
    { }

    public async Task<IEnumerable<WeatherForecast>> GetAsync(CancellationToken cancellationToken = default)
    {
        var uri = new Uri("example", UriKind.Relative)
            .Append("for", "testing")
            .WithParameter("queryparam1", "test_param");

        var result = await GetAsync<ApiServiceResult<IEnumerable<WeatherForecast>>>(uri, cancellationToken);
        result.EnsureSuccessStatusCode();

        return result.Value;
    }

    public async Task<IEnumerable<WeatherForecastDetails>> GetDetailsAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var uri = new Uri("example", UriKind.Relative)
            .Append("for", "testing")
            .WithParameter(nameof(date), date.ToString("yyyy-MM-dd"));

        var result = await GetAsync<ApiServiceResult<IEnumerable<WeatherForecastDetails>>>(uri, cancellationToken);
        result.EnsureSuccessStatusCode();

        return result.Value;
    }
}
