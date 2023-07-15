using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cympatic.Stub.Connectivity.Models;

public abstract class StubModel
{
    private readonly DateTimeOffset _createdDateTime = DateTimeOffset.UtcNow;

    [Required]
    public string Path { get; set; }

    public IDictionary<string, string> Query { get; set; }

    public IDictionary<string, IEnumerable<string>> Headers { get; set; }

    public DateTimeOffset GetCreatedDateTime()
    {
        return _createdDateTime;
    }
}
