using Cympatic.Extensions.Stub.Internal.Interfaces;

namespace Cympatic.Extensions.Stub.Models;

public record ReceivedRequest(string Path, string HttpMethod, IDictionary<string, string> Query, IDictionary<string, IEnumerable<string?>> Headers, string? Body, bool FoundMatchingResponse) 
    : IAutomaticExpireItem
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }
}
