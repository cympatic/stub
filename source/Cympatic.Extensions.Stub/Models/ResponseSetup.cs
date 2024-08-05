using Cympatic.Extensions.Stub.Internal.Interfaces;
using System.Net;

namespace Cympatic.Extensions.Stub.Models;

public record ResponseSetup : IAutomaticExpireItem
{
    public IList<string> HttpMethods { get; set; } = [];

    public HttpStatusCode ReturnStatusCode { get; set; }

    public Uri? Location { get; set; }

    public object? Response { get; set; }

    public int DelayInMilliseconds { get; set; }

    public string Path { get; set; } = string.Empty;

    public IDictionary<string, string> Query { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, IEnumerable<string?>> Headers { get; set; } = new Dictionary<string, IEnumerable<string?>>();

    public Guid Id { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }
}
