using Cympatic.Extensions.SpecFlow.Interfaces;
using Cympatic.Stub.Connectivity;
using Cympatic.Stub.Connectivity.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Cympatic.Extensions.SpecFlow
{
    public class StubServerHelper
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly SetupResponseApiService _setupResponseApiService;

        public StubServerHelper(ScenarioContext scenarioContext, SetupResponseApiService setupResponseApiService)
        {
            _scenarioContext = scenarioContext;
            _setupResponseApiService = setupResponseApiService;
            var (clientStub, identifierValue) = scenarioContext.GetStubInformation();

            _setupResponseApiService.SetClientStubIdentifierValue(clientStub, identifierValue);
        }

        public async Task AddPreparedDataToStubServer()
        {
            var types = typeof(IStubSpecFlowItem).GetAllClassesOf();

            foreach (var type in types)
            {
                if (_scenarioContext.TryGetValue(type.FullName, out var registeredItems) &&
                    registeredItems is IEnumerable<object> items)
                {
                    var stubSpecFlowItems = items.ToList().OfType<IStubSpecFlowItem>();

                    await AddRangeToStubServer(stubSpecFlowItems);
                    await AddItemToStubServer(stubSpecFlowItems);
                }
            }
        }

        public async Task AddRangeToStubServer(IEnumerable<IStubSpecFlowItem> stubSpecFlowtems)
        {
            var groupedList = stubSpecFlowtems
                .Where(item => item.ResponseToUrl is not null)
                .Select(item => new
                {
                    Uri = item.ResponseToUrl,
                    Item = item
                })
                .GroupBy(item => item.Uri)
                .ToList();

            foreach (var groupedItem in groupedList)
            {
                var payload = new List<object>();
                payload.AddRange(groupedItem.Select(obj => obj.Item));

                await _setupResponseApiService.AddOrUpdateAsync(
                    new ResponseModel
                    {
                        Path = groupedItem.Key.Path,
                        Query = groupedItem.Key.QueryParams,
                        Result = payload,
                        ReturnStatusCode = groupedItem.Key.ReturnHttpStatusCode
                    });
            }
        }

        public async Task AddItemToStubServer(IEnumerable<IStubSpecFlowItem> stubSpecFlowtems)
        {
            foreach (var item in stubSpecFlowtems.Where(item => item.ResponseToUrlScalar is not null).ToList())
            {
                await _setupResponseApiService.AddOrUpdateAsync(
                    new ResponseModel
                    {
                        Path = item.ResponseToUrlScalar.Path,
                        Query = item.ResponseToUrlScalar.QueryParams,
                        Result = item,
                        ReturnStatusCode = item.ResponseToUrlScalar.ReturnHttpStatusCode
                    });
            }
        }
    }
}
