using Cympatic.Extensions.SpecFlow;
using Cympatic.Extensions.SpecFlow.Attributes;
using Cympatic.Extensions.SpecFlow.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Cympatic.Stub.Example.Api.SpecFlow.Models
{
    [SpecFlowItemName("Weather forecast")]
    public class WeatherForecastComplex : WeatherForecast, IStubSpecFlowItem
    {
        public List<WeatherForecastDetails> Details { get; } = new();

        public string Alias { get; set; }

        public StubUrl ResponseToUrl => new(
            path: "example/{*wildcard}/testing",
            queryParams: new Dictionary<string, string>
            {
                { "queryparam1", "{*wildcard}" }
            },
            httpMethods: new List<string> { HttpMethod.Get.Method },
            returnHttpStatusCode: HttpStatusCode.OK);

        public StubUrl ResponseToUrlScalar => default;

        public void AddDetails(WeatherForecastDetails details)
        {
            Details.Add(details);
        }

        public void ConnectSpecFlowItem(ISpecFlowItem item)
        { }
    }
}
