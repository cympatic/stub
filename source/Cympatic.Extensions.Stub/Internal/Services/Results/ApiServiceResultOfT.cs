﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cympatic.Extensions.Stub.Internal.Services.Results;

internal class ApiServiceResult<TModel> : ApiServiceResult
    where TModel : class
{
    private readonly JsonSerializerOptions _options;

    public ApiServiceResult()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        _options.PropertyNameCaseInsensitive = true;
    }

    public TModel? Value { get; protected set; }

    public override async Task InitializeAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        await base.InitializeAsync(response, cancellationToken);

        if (string.IsNullOrWhiteSpace(Content))
        {
            return;
        }

        try
        {
            Value = JsonSerializer.Deserialize<TModel>(Content, _options);
        }
        catch
        {
            // Content was an invalid Json and should therefore be ingored
        }
    }
}
