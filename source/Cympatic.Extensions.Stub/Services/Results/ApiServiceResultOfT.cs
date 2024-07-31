using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cympatic.Extensions.Stub.Services.Results;

public class ApiServiceResult<TModel> : ApiServiceResult
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

        if (Content is null)
        {
            return;
        }

        Value = JsonSerializer.Deserialize<TModel>(Content, _options);
    }
}
