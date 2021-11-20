using Cympatic.Extensions.Stub.SpecFlow;
using Cympatic.Extensions.Stub.SpecFlow.Interfaces;
using Cympatic.Stub.Connectivity;
using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Connectivity.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Cympatic.Extensions.Stub.SpecFlow.Contexts
{
    public class StubContext : IAsyncDisposable, IDisposable
    {
        public string IdentifierValue { get; } = Guid.NewGuid().ToString("N");

        private readonly StubConnectivitySettings _stubConnectivitySettings;
        private readonly ItemContext _itemContext;
        private readonly SetupClientApiService _setupClientApiService;
        private readonly SetupResponseApiService _setupResponseApiService;
        private readonly VerifyRequestApiService _verifyRequestApiService;
        private bool initializedStubServices;

        public StubContext(
            IOptions<StubConnectivitySettings> options,
            ItemContext itemContext,
            SetupClientApiService setupClientApiService,
            SetupResponseApiService setupResponseApiService,
            VerifyRequestApiService verifyRequestApiService)
        {
            _stubConnectivitySettings = options.Value;
            _itemContext = itemContext;
            _setupClientApiService = setupClientApiService;
            _setupResponseApiService = setupResponseApiService;
            _verifyRequestApiService = verifyRequestApiService;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private async Task EnsureStubServicesAsync()
        {
            if (!initializedStubServices)
            {
                var client =
                        await _setupClientApiService.GetClientAsync(_stubConnectivitySettings.ClientName) ??
                        await _setupClientApiService.SetupAsync(_stubConnectivitySettings.ClientName, _stubConnectivitySettings.IdentifierHeaderName);

                _setupResponseApiService.SetClientStubIdentifierValue(client, IdentifierValue);
                _verifyRequestApiService.SetClientStubIdentifierValue(client, IdentifierValue);

                initializedStubServices = true;
            }
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (initializedStubServices)
            {
                await _setupResponseApiService.RemoveAsync();
                await _verifyRequestApiService.RemoveAsync();
            }
        }

        public IEnumerable<ISpecFlowItem> TransformItems([NotNull] string itemName, [NotNull] Table table)
        {
            return _itemContext.Transform(itemName, table);
        }

        public void RegisterItems([NotNull] string itemName, [NotNull] Table table)
        {
            _itemContext.Register(itemName, table);
        }

        public IList CreateItemContainer([NotNull] string itemName, [NotNull] Table table)
        {
            return _itemContext.CreateContainer(itemName, table);
        }

        public async Task PostItemsToStubServerAsync()
        {
            await EnsureStubServicesAsync();
            foreach (var item in _itemContext.Items.Values)
            {
                var stubSpecFlowItems = item.ToList().OfType<IStubSpecFlowItem>();

                await _setupResponseApiService.AddRangeToStubServerAsync(stubSpecFlowItems);
                await _setupResponseApiService.AddItemToStubServerAsync(stubSpecFlowItems);
            }
        }

        public async Task AddOrUpdateResponseAsync(ResponseModel model)
        {
            await EnsureStubServicesAsync();
            await _setupResponseApiService.AddOrUpdateAsync(model);
        }

        public async Task AddOrUpdateResponsesAsync(IEnumerable<ResponseModel> models)
        {
            await EnsureStubServicesAsync();
            await _setupResponseApiService.AddOrUpdateAsync(models);
        }

        public async Task<IEnumerable<RequestModel>> GetAllRequestsAsync()
        {
            await EnsureStubServicesAsync();
            return await _verifyRequestApiService.GetAsync();
        }

        public async Task<IEnumerable<RequestModel>> SearchRequestAsync(RequestSearchModel searchModel)
        {
            await EnsureStubServicesAsync();
            return await _verifyRequestApiService.SearchAsync(searchModel);
        }
    }
}
