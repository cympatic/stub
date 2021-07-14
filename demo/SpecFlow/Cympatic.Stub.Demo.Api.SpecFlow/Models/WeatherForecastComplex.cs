using Cympatic.Extensions.SpecFlow;
using Cympatic.Extensions.SpecFlow.Attributes;
using Cympatic.Extensions.SpecFlow.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Cympatic.Stub.Demo.Api.SpecFlow.Models
{
    [SpecFlowItem("WeatherForecast Complex")]
    public class WeatherForecastComplex : WeatherForecast, IStubSpecFlowItem
    {
        public string Alias { get; set; }

        public StubUrl ResponseToUrl => new(
            path: "demo/{*wildcard}/testing",
            queryParams: new Dictionary<string, string>
            {
                { "queryparam1", "{*wildcard}" }
            },
            httpMethods: new List<string> { HttpMethod.Get.Method },
            returnHttpStatusCode: HttpStatusCode.OK);

        public StubUrl ResponseToUrlScalar => default;

        public WeatherForecast Parent { get; private set; }

        public void ConnectSpecFlowItem(ISpecFlowItem item)
        {
            if (item is WeatherForecast weatherForecast)
            {
                Parent = weatherForecast;
            }
        }
    }
}
