using Cympatic.Extensions.SpecFlow;
using Cympatic.Stub.Connectivity;
using Cympatic.Stub.Connectivity.Interfaces;
using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Demo.Api.SpecFlow.Generators;
using Cympatic.Stub.Demo.Api.SpecFlow.Models;
using Cympatic.Stub.Demo.Api.SpecFlow.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Cympatic.Stub.Demo.Api.SpecFlow.Bindings
{
    [Binding]
    public sealed class WeatherForecastBinding
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly SetupResponseApiService _setupResponseApiService;
        private readonly VerifyRequestApiService _verifyRequestApiService;
        private readonly DemoApiService _demoApiService;

        public WeatherForecastBinding(
            ScenarioContext scenarioContext,
            SetupResponseApiService setupResponseApiService,
            VerifyRequestApiService verifyRequestApiService,
            DemoApiService demoApiService)
        {
            _scenarioContext = scenarioContext;
            _setupResponseApiService = setupResponseApiService;
            _verifyRequestApiService = verifyRequestApiService;
            _demoApiService = demoApiService;
        }

        [BeforeScenario(Order = 20)]
        public void BeforeScenarion()
        {
            var (clientStub, identifierValue) = _scenarioContext.GetStubInformation();

            _setupResponseApiService.SetClientStubIdentifierValue(clientStub, identifierValue);
            _verifyRequestApiService.SetClientStubIdentifierValue(clientStub, identifierValue);
        }

        [AfterScenario(Order = 20)]
        public async Task AfterScenario()
        {
            await _setupResponseApiService.RemoveAsync();
            await _verifyRequestApiService.RemoveAsync();
        }

        [Given(@"I have generate a random number of weahter forecasts")]
        public void GivenIHaveGenerateARandomNumberOfWeahterForecasts()
        {
            var generator = new WeatherForecastTestDataGenerator();
            _scenarioContext.Set(generator.GetData(), "expected");
        }

        [Given(@"I have setup the response the webapi call")]
        public async Task GivenIHaveSetupTheResponseTheWebapiCall()
        {
            var expected = _scenarioContext.Get<IEnumerable<WeatherForecast>>("expected");
            var responseModel = new ResponseModel
            {
                Path = "demo/{*wildcard}/testing",
                Query = new Dictionary<string, string>
                {
                    { "queryparam1", "{*wildcard}" }
                },
                HttpMethods = new List<string> { HttpMethod.Get.Method },
                ReturnStatusCode = HttpStatusCode.OK,
                Result = expected
            };

            await _setupResponseApiService.AddOrUpdateAsync(responseModel);
        }

        [When(@"I request for weather forecasts")]
        public async Task WhenIRequestWeatherForecasts()
        {
            var (_, identifierValue) = _scenarioContext.GetStubInformation();

            // identifierValue is add to the Request.Headers, but is done in the calling method of the DemoApiService
            var actual = await _demoApiService.GetForecasts(identifierValue);
            _scenarioContext.Set(actual, "actual");
        }

        [Then(@"the result should be equal to the weather forecasts")]
        public void ThenTheResultShouldBeEqualToTheWeatherForecasts()
        {
            var expected = _scenarioContext.Get<IEnumerable<WeatherForecast>>("expected");
            var actual = _scenarioContext.Get<IEnumerable<WeatherForecast>>("actual");

            actual.Should().BeEquivalentTo(expected);
        }

        [Then(@"the stub server is called once")]
        public async Task ThenTheStubServerIsCalledOnce()
        {
            var searchModel = new RequestSearchModel
            {
                Path = "demo/for/testing",
                Query = new Dictionary<string, string>
                {
                    { "queryparam1", "{*wildcard}" }
                },
                HttpMethods = new List<string> { HttpMethod.Get.Method }
            };
            var requests = await _verifyRequestApiService.SearchAsync(searchModel);
            requests.Should().HaveCount(1);
        }
    }
}
