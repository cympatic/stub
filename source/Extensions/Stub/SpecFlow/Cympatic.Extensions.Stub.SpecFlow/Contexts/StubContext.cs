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
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Cympatic.Extensions.Stub.SpecFlow.Contexts;

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

    public IEnumerable<ISpecFlowItem> TransformItems([NotNull] string itemName, [NotNull] Table table)
    {
        return _itemContext.Transform(itemName, table);
    }

    public void RegisterItems([NotNull] string itemName, [NotNull] Table table)
    {
        _itemContext.Register(itemName, table);
    }

    public IList CreateItemList([NotNull] string itemName, [NotNull] Table table)
    {
        return _itemContext.CreateList(itemName, table);
    }

    public async Task PostItemsToStubServerAsync(CancellationToken cancellationToken = default)
    {
        await EnsureStubServicesAsync(cancellationToken);
        foreach (var item in _itemContext.Items.Values)
        {
            var stubSpecFlowItems = item.ToList().OfType<IStubSpecFlowItem>();

            await _setupResponseApiService.AddRangeToStubServerAsync(stubSpecFlowItems, cancellationToken: cancellationToken);
            await _setupResponseApiService.AddItemToStubServerAsync(stubSpecFlowItems, cancellationToken: cancellationToken);
        }
    }

    public async Task AddOrUpdateResponseAsync([NotNull] ResponseModel model, CancellationToken cancellationToken = default)
    {
        await EnsureStubServicesAsync(cancellationToken);
        await _setupResponseApiService.AddOrUpdateAsync(model, cancellationToken);
    }

    public async Task AddOrUpdateResponsesAsync([NotNull] IEnumerable<ResponseModel> models, CancellationToken cancellationToken = default)
    {
        await EnsureStubServicesAsync(cancellationToken);
        await _setupResponseApiService.AddOrUpdateAsync(models, cancellationToken);
    }

    public async Task<IEnumerable<RequestModel>> GetAllRequestsAsync(CancellationToken cancellationToken = default)
    {
        await EnsureStubServicesAsync(cancellationToken);
        return await _verifyRequestApiService.GetAsync(cancellationToken);
    }

    public Task<IEnumerable<RequestModel>> SearchRequestAsync([NotNull] StubUrl stubUrl, CancellationToken cancellationToken = default)
    {
        return _verifyRequestApiService.SearchAsync(new RequestSearchModel
        {
            Path = stubUrl.Path,
            Query = stubUrl.QueryParams,
            HttpMethods = stubUrl.HttpMethods.ToList()
        }, cancellationToken);
    }

    public async Task<IEnumerable<RequestModel>> SearchRequestAsync([NotNull] RequestSearchModel searchModel, CancellationToken cancellationToken = default)
    {
        await EnsureStubServicesAsync(cancellationToken);
        return await _verifyRequestApiService.SearchAsync(searchModel, cancellationToken);
    }

    protected virtual async ValueTask DisposeAsyncCore(CancellationToken cancellationToken = default)
    {
        if (initializedStubServices)
        {
            await _setupResponseApiService.RemoveAsync(cancellationToken);
            await _verifyRequestApiService.RemoveAsync(cancellationToken);
        }
    }

    private async Task EnsureStubServicesAsync(CancellationToken cancellationToken)
    {
        if (!initializedStubServices)
        {
            var client =
                await _setupClientApiService.GetClientAsync(_stubConnectivitySettings.Name, cancellationToken) ??
                await _setupClientApiService.SetupAsync(_stubConnectivitySettings.Name, _stubConnectivitySettings.IdentificationHeaderName, cancellationToken);

            _setupResponseApiService.SetClientStubIdentifierValue(client, IdentifierValue, _stubConnectivitySettings.UseIdentificationHeader);
            _verifyRequestApiService.SetClientStubIdentifierValue(client, IdentifierValue, _stubConnectivitySettings.UseIdentificationHeader);

            initializedStubServices = true;
        }
    }
}
