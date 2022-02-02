using Cympatic.Extensions.Http.Services.Results;
using Cympatic.Extensions.Stub.SpecFlow;
using Cympatic.Extensions.Stub.SpecFlow.Contexts;
using Cympatic.Stub.Example.Api.SpecFlow.Models;
using Cympatic.Stub.Example.Api.SpecFlow.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Cympatic.Stub.Example.Api.SpecFlow.Bindings
{
    [Binding]
    public sealed class WeatherForecastComplexBinding
    {
        private readonly StubContext _stubContext;
        private readonly ExampleApiService _exampleApiService;

        private ApiServiceResult<IEnumerable<WeatherForecast>> _actual;

        public WeatherForecastComplexBinding(StubContext stubContext, ExampleApiService exampleApiService)
        {
            _stubContext = stubContext;
            _exampleApiService = exampleApiService;
        }

        [BeforeScenario("Complex", Order = 20)]
        public void BeforeScenarion()
        {
            _exampleApiService.SetIdentifierValue(_stubContext.IdentifierValue);
        }

        [Given(@"multiple '(.*)' items containing the following values")]
        public void GivenMultipleItemsContainingTheFollowingValues(string itemName, Table table)
        {
            _stubContext.RegisterItems(itemName, table);
        }

        [Given(@"the 'Stub Server' is prepared")]
        [When(@"the 'Stub Server' is prepared")]
        public async Task WhenTheStubServerIsPrepared()
        {
            await _stubContext.PostItemsToStubServerAsync();
        }

        [When(@"the 'Weather forecast' service is requested for weather forecasts")]
        public async Task WhenTheServiceIsRequestedForWeatherForecasts()
        {
            _actual = await _exampleApiService.GetForecastsAsync();
            _actual.EnsureSuccessStatusCode();
        }

        [Then(@"the request returned one or more '(.*)' items containing the following values")]
        public void ThenTheRequestReturnedOneOrMoreItemsContainingTheFollowingValues(string itemName, Table table)
        {
            var expected = table.CreateSet(() => 
            {
                var type = itemName.GetSpecFlowItemType();
                var instance = Activator.CreateInstance(type);
                table.FillInstance(instance);

                return instance;
            });

            _actual.ValidateResult(expected);
        }

        [Then(@"the request returned a list with one or more '(.*)' items containing the following values")]
        public void ThenTheRequestReturnedAListWithOneOrMoreItemsContainingTheFollowingValues(string itemName, Table table)
        {
            var expected = _stubContext.CreateItemList(itemName, table);

            _actual.ValidateResult(expected);
        }
    }
}
