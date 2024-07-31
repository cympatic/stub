using Cympatic.Extensions.Stub.Internal.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Cympatic.Extensions.Stub.Models;

public record ReceivedRequest(
    string? Path,
    string? HttpMethod,
    IDictionary<string, string>? Query,
    IDictionary<string, IEnumerable<string?>>? Headers,
    string? Body,
    bool FoundMatchingResponse) 
    : IAutomaticExpireItem
{
    private readonly DateTimeOffset _createdDateTime = DateTimeOffset.UtcNow;

    public readonly Guid Id = Guid.NewGuid();

    public string? HttpMethod { get; set; } = HttpMethod;

    public string? Body { get; set; } = Body;

    public bool FoundMatchingResponse { get; set; } = FoundMatchingResponse;
    
    [Required]
    public string? Path { get; set; } = Path;

    public IDictionary<string, string>? Query { get; set; } = Query;

    public IDictionary<string, IEnumerable<string?>>? Headers { get; set; } = Headers;

    public DateTimeOffset CreatedDateTime => _createdDateTime;
}
