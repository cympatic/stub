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

            _setupResponseApiService.SetIdentifierValue(clientStub, identifierValue);
        }

        public async Task AddRange(IEnumerable<StubItem> subItems)
        {
            var (clientStub, _) = _scenarioContext.GetStubInformation();
            var groupedList = subItems
                .Where(item => item.ResponseToUrl is not null)
                .Select(item => new
                {
                    Uri = item.ResponseToUrl,
                    Item = item
                })
                .GroupBy(item => item.Uri);

            foreach (var groupedItem in groupedList)
            {
                var payload = new List<object>();
                payload.AddRange(groupedItem.Select(obj => obj.Item.Items));

                await _setupResponseApiService.AddOrUpdateAsync(clientStub,
                    new ResponseModel
                    {
                        Path = groupedItem.Key.Path,
                        Query = groupedItem.Key.QueryParams,
                        Result = payload,
                        ReturnStatusCode = groupedItem.Key.ReturnHttpStatusCode
                    });
            }
        }

        public async Task AddItem(IEnumerable<StubItem> subItems)
        {
            var (clientStub, _) = _scenarioContext.GetStubInformation();
            var groupedList = subItems
                .Where(item => item.ResponseToUrl is not null)
                .Select(item => new
                {
                    Uri = item.ResponseToUrl,
                    Item = item
                })
                .GroupBy(item => item.Uri);

            foreach (var groupedItem in groupedList)
            {
                var payload = new List<object>();
                payload.AddRange(groupedItem.Select(obj => obj.Item.Items.FirstOrDefault()));

                await _setupResponseApiService.AddOrUpdateAsync(clientStub,
                    new ResponseModel
                    {
                        Path = groupedItem.Key.Path,
                        Query = groupedItem.Key.QueryParams,
                        Result = payload,
                        ReturnStatusCode = groupedItem.Key.ReturnHttpStatusCode
                    });
            }
        }
    }
}
