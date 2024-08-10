using Cympatic.Extensions.Stub.Models;

namespace Cympatic.Extensions.Stub.Internal.Interfaces;

internal interface IReceivedRequestCollection
{
    void Add(ReceivedRequest item);

    IEnumerable<ReceivedRequest> All();

    IEnumerable<ReceivedRequest> Find(ReceivedRequestSearchParams searchParams);

    void Clear();
}
