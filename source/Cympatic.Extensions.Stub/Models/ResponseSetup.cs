using Cympatic.Extensions.Stub.Internal.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Cympatic.Extensions.Stub.Models;

public record ResponseSetup : IAutomaticExpireItem
{
    private readonly DateTimeOffset _createdDateTime = DateTimeOffset.UtcNow;

    public readonly Guid Id = Guid.NewGuid();

    public IList<string>? HttpMethods { get; set; }

    public HttpStatusCode ReturnStatusCode { get; set; }

    public Uri? Location { get; set; }

    public object? Response { get; set; }

    public int DelayInMilliseconds { get; set; }

    [Required]
    public string? Path { get; set; }

    public IDictionary<string, string>? Query { get; set; }

    public IDictionary<string, IEnumerable<string?>>? Headers { get; set; }

    public DateTimeOffset CreatedDateTime => _createdDateTime;
}
