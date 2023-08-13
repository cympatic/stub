using Cympatic.Extensions.Http.Services;
using Cympatic.Extensions.Http.Services.Results;
using Cympatic.Stub.Connectivity.Settings;
using Cympatic.Stub.Example.Api.SpecFlow.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Stub.Example.Api.SpecFlow.Services;

public class ExampleApiService : ApiService
{
    private readonly StubConnectivitySettings _options;

    public ExampleApiService(HttpClient httpClient, IOptions<StubConnectivitySettings> options) 
        : base(httpClient)
    { 
        _options = options.Value;
    }

    public void SetIdentifierValue(string identifierValue)
    {
        // NOTE:
        // For example purpose only the identifierValue is added to the Request.Headers
        // In real life use a headername should be chosen that can be manipulated,
        // can contain an unique value, and is passed into all calls with the chain
        if (_options.UseIdentificationHeader)
        {
            HttpClient.DefaultRequestHeaders.Add(_options.IdentificationHeaderName, identifierValue);
        }
    }

    public Task<ApiServiceResult<IEnumerable<WeatherForecast>>> GetForecastsAsync(CancellationToken cancellationToken = default)
    {
        var uri = new Uri("weatherforecast", UriKind.Relative);

        return GetAsync<ApiServiceResult<IEnumerable<WeatherForecast>>>(uri, cancellationToken);
    }
}
