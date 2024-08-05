﻿using Cympatic.Extensions.Stub.Internal.Interfaces;

namespace Cympatic.Extensions.Stub.Models;

public record ReceivedRequest : IAutomaticExpireItem
{
    public ReceivedRequest(string path, string httpMethod, IDictionary<string, string> query, IDictionary<string, IEnumerable<string?>> headers, string? body, bool foundMatchingResponse)
    {
        Path = path;
        HttpMethod = httpMethod;
        Query = query;
        Headers = headers;
        Body = body;
        FoundMatchingResponse = foundMatchingResponse;
    }

    public string Path { get; set; }

    public string HttpMethod { get; set; }

    public IDictionary<string, string> Query { get; set; }

    public IDictionary<string, IEnumerable<string?>> Headers { get; set; }

    public string? Body { get; set; }

    public bool FoundMatchingResponse { get; set; }

    public Guid Id { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }
}
