using Cympatic.Extensions.SpecFlow;
using Cympatic.Extensions.SpecFlow.Interfaces;
using Cympatic.Stub.Connectivity;
using Cympatic.Stub.Demo.Api.SpecFlow.Services;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Cympatic.Stub.Demo.Api.SpecFlow.Bindings
{
    [Binding]
    public sealed class WeatherForecastComplexBinding
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly VerifyRequestApiService _verifyRequestApiService;
        private readonly DemoApiService _demoApiService;
        private readonly StubServerHelper _stubServerHelper;

        public WeatherForecastComplexBinding(
            ScenarioContext scenarioContext,
            VerifyRequestApiService verifyRequestApiService,
            DemoApiService demoApiService,
            StubServerHelper stubServerHelper)
        {
            _scenarioContext = scenarioContext;
            _verifyRequestApiService = verifyRequestApiService;
            _demoApiService = demoApiService;
            _stubServerHelper = stubServerHelper;
        }

        [BeforeScenario(Order = 20)]
        public void BeforeScenarion()
        {
            var (clientStub, identifierValue) = _scenarioContext.GetStubInformation();

            _verifyRequestApiService.SetClientStubIdentifierValue(clientStub, identifierValue);

            _demoApiService.SetIdentifierValue(identifierValue);
        }

        [AfterScenario(Order = 20)]
        public async Task AfterScenario()
        {
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
            await _stubServerHelper.AddPreparedDataToStubServer();
        }

        [When(@"the 'Weather forecast' service is requested for weather forecasts")]
        public async Task WhenTheServiceIsRequestedForWeatherForecasts()
        {
            var result = await _demoApiService.GetForecastsAsync();
            _scenarioContext.Set(result, $"{_demoApiService.GetType().FullName}Result");
        }

        [Then(@"the request returned httpCode '(.*)'")]
        public void ThenTheRequestReturnedHttpCode(string httpStatusCode)
        {
            var statusCode = httpStatusCode.ToEnum<HttpStatusCode>();

            var result = _scenarioContext.Get<IApiServiceResult>($"{_demoApiService.GetType().FullName}Result");

            result.StatusCode.Should().BeEquivalentTo(statusCode);
        }

        [Then(@"the request returned one or more '(.*)' items containing the following values")]
        public void ThenTheRequestReturnedOneOrMoreItemsContainingTheFollowingValues(string itemName, Table table)
        {
            var type = itemName.GetSpecFlowItemType();
            var expected = table.TransformToSpecFlowItems(type, _scenarioContext);
            var result = _scenarioContext.Get<IApiServiceResult>($"{_demoApiService.GetType().FullName}Result");

            result.ValidateResult(expected);
        }

    }
}
