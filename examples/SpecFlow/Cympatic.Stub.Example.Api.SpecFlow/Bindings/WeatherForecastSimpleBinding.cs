using Cympatic.Extensions.SpecFlow;
using Cympatic.Stub.Connectivity;
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
        private readonly ScenarioContext _scenarioContext;
        private readonly SetupResponseApiService _setupResponseApiService;
        private readonly VerifyRequestApiService _verifyRequestApiService;
        private readonly ExampleApiService _exampleApiService;

        public WeatherForecastSimpleBinding(
            ScenarioContext scenarioContext,
            SetupResponseApiService setupResponseApiService,
            VerifyRequestApiService verifyRequestApiService,
            ExampleApiService exampleApiService)
        {
            _scenarioContext = scenarioContext;
            _setupResponseApiService = setupResponseApiService;
            _verifyRequestApiService = verifyRequestApiService;
            _exampleApiService = exampleApiService;
        }

        [BeforeScenario("Simple", Order = 20)]
        public void BeforeScenarion()
        {
            var (clientStub, identifierValue) = _scenarioContext.GetStubInformation();

            _setupResponseApiService.SetClientStubIdentifierValue(clientStub, identifierValue);
            _verifyRequestApiService.SetClientStubIdentifierValue(clientStub, identifierValue);

            _exampleApiService.SetIdentifierValue(identifierValue);
        }

        [AfterScenario("Simple", Order = 20)]
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
                Path = "example/{*wildcard}/testing",
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
            var actual = await _exampleApiService.GetForecastsAsync();
            _scenarioContext.Set(actual.Value, "actual");
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
                Path = "example/for/testing",
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
