using Cympatic.Extensions.SpecFlow.Interfaces;
using Cympatic.Stub.Connectivity;
using Cympatic.Stub.Connectivity.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Cympatic.Extensions.SpecFlow
{
    public static class ScenarioContextExtensions
    {
        public static (IClientStub clientStub, string identifierValue) GetStubInformation(this ScenarioContext scenarioContext)
        {
            if (scenarioContext == null)
            {
                throw new ArgumentNullException(nameof(scenarioContext));
            }

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

        public static async Task PostStubSpecFlowItemsToStubServerAsync(this ScenarioContext scenarioContext, [NotNull] SetupResponseApiService setupResponseApiService)
        {
            if (scenarioContext == null)
            {
                throw new ArgumentNullException(nameof(scenarioContext));
            }

            var types = typeof(IStubSpecFlowItem).GetAllClassesOf();
            foreach (var type in types)
            {
                if (scenarioContext.TryGetValue(type.FullName, out var registeredItems) &&
                    registeredItems is IEnumerable<object> items)
                {
                    var stubSpecFlowItems = items.ToList().OfType<IStubSpecFlowItem>();

                    await setupResponseApiService.AddRangeToStubServerAsync(stubSpecFlowItems);
                    await setupResponseApiService.AddItemToStubServerAsync(stubSpecFlowItems);
                }
            }
        }
    }
}
