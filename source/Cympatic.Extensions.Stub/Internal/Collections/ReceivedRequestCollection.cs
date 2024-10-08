﻿using Cympatic.Extensions.Stub.Internal.Interfaces;
using Cympatic.Extensions.Stub.Internal.Utilities;
using Cympatic.Extensions.Stub.Models;

namespace Cympatic.Extensions.Stub.Internal.Collections;

internal sealed class ReceivedRequestCollection : AutomaticExpireCollection<ReceivedRequest>, IReceivedRequestCollection
{
    public IEnumerable<ReceivedRequest> Find(ReceivedRequestSearchParams searchParams)
    {
        return Find(item => item.FoundMatchingResponse && IsMatching(item, searchParams.HttpMethods, searchParams.Path ?? string.Empty, searchParams.Query)).ToArray();
    }

    private static bool IsMatching(ReceivedRequest item, IList<string> httpMethods, string path, IDictionary<string, string> query)
    {
        return SearchableStubItemUtility.IsMatching(httpMethods, item.HttpMethod, item.Path, path, item.Query, query);
    }
}
