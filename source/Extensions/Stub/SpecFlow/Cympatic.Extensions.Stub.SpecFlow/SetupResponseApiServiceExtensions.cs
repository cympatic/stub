using Cympatic.Extensions.Stub.SpecFlow.Interfaces;
using Cympatic.Stub.Connectivity;
using Cympatic.Stub.Connectivity.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Extensions.Stub.SpecFlow;

public static class SetupResponseApiServiceExtensions
{
    public static async Task AddRangeToStubServerAsync(this SetupResponseApiService setupResponseApiService, 
        [NotNull] IEnumerable<IStubSpecFlowItem> stubSpecFlowtems, 
        CancellationToken cancellationToken = default)
    {
        if (setupResponseApiService == null)
        {
            throw new ArgumentNullException(nameof(setupResponseApiService));
        }

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

            await setupResponseApiService.AddOrUpdateAsync(new ResponseModel
                {
                    Path = groupedItem.Key.Path,
                    Query = groupedItem.Key.QueryParams,
                    Result = payload,
                    ReturnStatusCode = groupedItem.Key.ReturnHttpStatusCode
                }, cancellationToken);
        }
    }

    public static async Task AddItemToStubServerAsync(this SetupResponseApiService setupResponseApiService, 
        [NotNull] IEnumerable<IStubSpecFlowItem> stubSpecFlowtems, 
        CancellationToken cancellationToken = default)
    {
        if (setupResponseApiService == null)
        {
            throw new ArgumentNullException(nameof(setupResponseApiService));
        }

        foreach (var item in stubSpecFlowtems.Where(item => item.ResponseToUrlScalar is not null).ToList())
        {
            await setupResponseApiService.AddOrUpdateAsync(new ResponseModel
                {
                    Path = item.ResponseToUrlScalar.Path,
                    Query = item.ResponseToUrlScalar.QueryParams,
                    Result = item,
                    ReturnStatusCode = item.ResponseToUrlScalar.ReturnHttpStatusCode
                }, cancellationToken);
        }
    }
}
