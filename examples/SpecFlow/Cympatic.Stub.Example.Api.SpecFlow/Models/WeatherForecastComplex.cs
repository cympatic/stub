using Cympatic.Extensions.Stub.SpecFlow;
using Cympatic.Extensions.Stub.SpecFlow.Attributes;
using Cympatic.Extensions.Stub.SpecFlow.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Cympatic.Stub.Example.Api.SpecFlow.Models;

[SpecFlowItemName("Weather forecast")]
public class WeatherForecastComplex : WeatherForecast, IStubSpecFlowItem
{
    [Ignore]
    public string Alias { get; set; }

    [Ignore]
    public StubUrl ResponseToUrl => new(
        path: "example/{*wildcard}/testing",
        queryParams: new Dictionary<string, string>
        {
            { "queryparam1", "{*wildcard}" }
        },
        httpMethods: new List<string> { HttpMethod.Get.Method },
        returnHttpStatusCode: HttpStatusCode.OK);

    [Ignore]
    public StubUrl ResponseToUrlScalar => default;

    public void AddDetails(WeatherForecastDetails details)
    {
        Details.Add(details);
    }

    public void ConnectSpecFlowItem(ISpecFlowItem item)
    { }
}
