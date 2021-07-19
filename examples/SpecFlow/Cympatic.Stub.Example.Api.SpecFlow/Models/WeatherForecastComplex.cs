using Cympatic.Extensions.SpecFlow;
using Cympatic.Extensions.SpecFlow.Attributes;
using Cympatic.Extensions.SpecFlow.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Cympatic.Stub.Example.Api.SpecFlow.Models
{
    [SpecFlowItem("WeatherForecast Stub")]
    public class WeatherForecastComplex : WeatherForecast, IStubSpecFlowItem
    {
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

        public void ConnectSpecFlowItem(ISpecFlowItem item)
        { }
    }
}
