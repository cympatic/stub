using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Extensions.SpecFlow.Services.Results
{
    public abstract class ApiServiceResult<TModel> : ApiServiceResult
        where TModel : class
    {
        public TModel Value { get; protected set; }

        public override async Task InitializeAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            await base.InitializeAsync(response, cancellationToken);

            try
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.PropertyNameCaseInsensitive = true;

                Value = JsonSerializer.Deserialize<TModel>(Content, options);
            }
            catch (Exception)
            {
                Value = default;
            }
        }
    }
}
