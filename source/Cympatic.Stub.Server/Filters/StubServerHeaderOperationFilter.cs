using Cympatic.Stub.Abstractions.Constants;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cympatic.Stub.Server.Filters
{
    public class StubServerHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Tags.Any(tag => tag.Name.Equals("SetupClient", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = Defaults.IdentifierHeaderName,
                Description = "Session identifier per client",
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString(Guid.NewGuid().ToString("N"))
                }
            });
        }
    }
}
