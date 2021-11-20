using Cympatic.Extensions.Http.Interfaces;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Extensions.Http.Services.Results
{
    public class ApiServiceResult<TModel> : ApiServiceResult, IApiServiceResultValue
        where TModel : class
    {
        public TModel Value { get; protected set; }

        object IApiServiceResultValue.Value => Value;

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
