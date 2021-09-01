using Cympatic.Extensions.SpecFlow;
using Cympatic.Extensions.SpecFlow.Interfaces;
using Cympatic.Stub.Connectivity;
using Cympatic.Stub.Example.Api.SpecFlow.Services;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Cympatic.Stub.Example.Api.SpecFlow.Bindings
{
    [Binding]
    public sealed class WeatherForecastComplexBinding
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly SetupResponseApiService _setupResponseApiService;
        private readonly VerifyRequestApiService _verifyRequestApiService;
        private readonly ExampleApiService _exampleApiService;

        public WeatherForecastComplexBinding(
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

        [BeforeScenario("Complex", Order = 20)]
        public void BeforeScenarion()
        {
            var (clientStub, identifierValue) = _scenarioContext.GetStubInformation();

            _setupResponseApiService.SetClientStubIdentifierValue(clientStub, identifierValue);
            _verifyRequestApiService.SetClientStubIdentifierValue(clientStub, identifierValue);

            _exampleApiService.SetIdentifierValue(identifierValue);
        }

        [AfterScenario("Complex", Order = 20)]
        public async Task AfterScenario()
        {
            await _setupResponseApiService.RemoveAsync();
            await _verifyRequestApiService.RemoveAsync();
        }


        [Given(@"multiple '(.*)' items containing the following values")]
        public void GivenMultipleItemsContainingTheFollowingValues(string itemName, Table table)
        {
            var type = itemName.GetSpecFlowItemType();
            table.TransformToSpecFlowItemsAndRegister(type, _scenarioContext);
        }

        [Given(@"the 'Stub Server' is prepared")]
        [When(@"the 'Stub Server' is prepared")]
        public async Task WhenTheStubServerIsPrepared()
        {
            await _scenarioContext.PostStubSpecFlowItemsToStubServerAsync(_setupResponseApiService);
        }

        [When(@"the 'Weather forecast' service is requested for weather forecasts")]
        public async Task WhenTheServiceIsRequestedForWeatherForecasts()
        {
            var result = await _exampleApiService.GetForecastsAsync();
            _scenarioContext.Set(result, $"{_exampleApiService.GetType().FullName}Result");
        }

        [Then(@"the request returned httpCode '(.*)'")]
        public void ThenTheRequestReturnedHttpCode(string httpStatusCode)
        {
            var statusCode = httpStatusCode.ToEnum<HttpStatusCode>();

            var actual = _scenarioContext.Get<IApiServiceResult>($"{_exampleApiService.GetType().FullName}Result");

            actual.StatusCode.Should().Be(statusCode);
        }

        [Then(@"the request returned one or more '(.*)' items containing the following values")]
        public void ThenTheRequestReturnedOneOrMoreItemsContainingTheFollowingValues(string itemName, Table table)
        {
            var type = itemName.GetSpecFlowItemType();
            var expected = table.TransformToSpecFlowItems(type, _scenarioContext);
            var actual = _scenarioContext.Get<IApiServiceResult>($"{_exampleApiService.GetType().FullName}Result");

            actual.ValidateResult(expected);
        }

        [Then(@"the request returned a container with one or more '(.*)' items containing the following values")]
        public void ThenTheRequestReturnedAContainerWithOneOrMoreItemsContainingTheFollowingValues(string itemName, Table table)
        {
            var type = itemName.GetSpecFlowItemType();
            var expected = table.CreateContainer(type, _scenarioContext);

            var actual = _scenarioContext.Get<IApiServiceResult>($"{_exampleApiService.GetType().FullName}Result");


            actual.ValidateResult(expected);
        }
    }
}
