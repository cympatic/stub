using Cympatic.Extensions.Stub.SpecFlow.Contexts;
using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Example.Api.SpecFlow.Generators;
using Cympatic.Stub.Example.Api.SpecFlow.Models;
using Cympatic.Stub.Example.Api.SpecFlow.Services;
using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Cympatic.Stub.Example.Api.SpecFlow.Bindings
{
    [Binding]
    public sealed class WeatherForecastSimpleBinding
    {
        private readonly StubContext _stubContext;
        private readonly ExampleApiService _exampleApiService;

        private IEnumerable<WeatherForecast> _expected;
        private IEnumerable<WeatherForecast> _actual;

        public WeatherForecastSimpleBinding(StubContext stubContext, ExampleApiService exampleApiService)
        {
            _stubContext = stubContext;
            _exampleApiService = exampleApiService;
        }

        [BeforeScenario("Simple", Order = 20)]
        public void BeforeScenarion()
        {
            _exampleApiService.SetIdentifierValue(_stubContext.IdentifierValue);
        }

        [Given(@"I have generate a random number of weahter forecasts")]
        public void GivenIHaveGenerateARandomNumberOfWeahterForecasts()
        {
            var generator = new WeatherForecastTestDataGenerator();
            _expected = generator.GetData();
        }

        [Given(@"I have setup the response the webapi call")]
        public async Task GivenIHaveSetupTheResponseTheWebapiCall()
        {
            var responseModel = new ResponseModel
            {
                Path = "example/{*wildcard}/testing",
                Query = new Dictionary<string, string>
                {
                    { "queryparam1", "{*wildcard}" }
                },
                HttpMethods = new List<string> { HttpMethod.Get.Method },
                ReturnStatusCode = HttpStatusCode.OK,
                Result = _expected
            };

            await _stubContext.AddOrUpdateResponseAsync(responseModel);
        }

        [When(@"I request for weather forecasts")]
        public async Task WhenIRequestWeatherForecasts()
        {
            var result = await _exampleApiService.GetForecastsAsync();
            result.EnsureSuccessStatusCode();

            _actual = result.Value;
        }

        [Then(@"the result should be equal to the weather forecasts")]
        public void ThenTheResultShouldBeEqualToTheWeatherForecasts()
        {
            _actual.Should().BeEquivalentTo(_expected);
        }

        [Then(@"the stub server is called once")]
        public async Task ThenTheStubServerIsCalledOnce()
        {
            var searchModel = new RequestSearchModel
            {
                Path = "example/for/testing",
                Query = new Dictionary<string, string>
                {
                    { "queryparam1", "{*wildcard}" }
                },
                HttpMethods = new List<string> { HttpMethod.Get.Method }
            };
            var requests = await _stubContext.SearchRequestAsync(searchModel);
            requests.Should().HaveCount(1);
        }
    }
}
