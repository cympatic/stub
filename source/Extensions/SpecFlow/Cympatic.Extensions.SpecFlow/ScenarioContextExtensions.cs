using Cympatic.Stub.Connectivity.Interfaces;
using System;
using TechTalk.SpecFlow;

namespace Cympatic.Extensions.SpecFlow
{
    public static class ScenarioContextExtensions
    {
        public static (IClientStub clientStub, string identifierValue) GetStubInformation(this ScenarioContext scenarioContext)
        {
            if (!scenarioContext.TryGetValue<IClientStub>(out var clientStub))
            {
                throw new InvalidOperationException($"nameof(IClientStub) not found in ScenarioContext");
            }

            if (!scenarioContext.TryGetValue<string>(clientStub.IdentifierHeaderName, out var identifierValue))
            {
                throw new InvalidOperationException($"nameof(IClientStub) not found in ScenarioContext");
            }

            return (clientStub, identifierValue);
        }
    }
}
