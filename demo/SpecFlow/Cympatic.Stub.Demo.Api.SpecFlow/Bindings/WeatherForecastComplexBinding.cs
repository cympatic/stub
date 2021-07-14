using Cympatic.Extensions.SpecFlow;
using Cympatic.Stub.Connectivity;
using Cympatic.Stub.Demo.Api.SpecFlow.Services;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Cympatic.Stub.Demo.Api.SpecFlow.Bindings
{
    [Binding]
    public sealed class WeatherForecastComplexBinding
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly SetupResponseApiService _setupResponseApiService;
        private readonly VerifyRequestApiService _verifyRequestApiService;
        private readonly DemoApiService _demoApiService;
        private readonly StubServerHelper _stubServerHelper;

        public WeatherForecastComplexBinding(
            ScenarioContext scenarioContext,
            SetupResponseApiService setupResponseApiService,
            VerifyRequestApiService verifyRequestApiService,
            DemoApiService demoApiService,
            StubServerHelper stubServerHelper)
        {
            _scenarioContext = scenarioContext;
            _setupResponseApiService = setupResponseApiService;
            _verifyRequestApiService = verifyRequestApiService;
            _demoApiService = demoApiService;
            _stubServerHelper = stubServerHelper;
        }

        [BeforeScenario(Order = 20)]
        public void BeforeScenarion()
        {
            var (clientStub, identifierValue) = _scenarioContext.GetStubInformation();

            _setupResponseApiService.SetClientStubIdentifierValue(clientStub, identifierValue);
            _verifyRequestApiService.SetClientStubIdentifierValue(clientStub, identifierValue);

            _demoApiService.SetIdentifierValue(identifierValue);
        }

        [AfterScenario(Order = 20)]
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
            await _stubServerHelper.AddPreparedDataToStubServer();
        }

    }
}
