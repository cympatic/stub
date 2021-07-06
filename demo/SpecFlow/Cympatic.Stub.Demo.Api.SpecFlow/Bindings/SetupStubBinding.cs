using Cympatic.Stub.Connectivity;
using Cympatic.Stub.Connectivity.Interfaces;
using Cympatic.Stub.Connectivity.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Cympatic.Stub.Demo.Api.SpecFlow.Bindings
{
    [Binding]
    public sealed class SetupStubBinding
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly StubConnectivitySettings _stubConnectivitySettings;
        private readonly SetupClientApiService _setupClientApiService;

        public SetupStubBinding(ScenarioContext scenarioContext, IOptions<StubConnectivitySettings> options, SetupClientApiService setupClientApiService)
        {
            _scenarioContext = scenarioContext;
            _stubConnectivitySettings = options.Value;
            _setupClientApiService = setupClientApiService;
        }

        [BeforeScenario(Order = 10)]
        public async Task BeforeScenario()
        {
            var clientStub = await _setupClientApiService.GetClientAsync(_stubConnectivitySettings.ClientName);
            if (clientStub == null)
            {
                clientStub = await _setupClientApiService.SetupAsync(_stubConnectivitySettings.ClientName, _stubConnectivitySettings.IdentifierHeaderName);
            }

            var identifierValue = Guid.NewGuid().ToString("N");

            _scenarioContext.Set(clientStub);
            _scenarioContext.Set(identifierValue, clientStub.IdentifierHeaderName);
        }
    }
}
