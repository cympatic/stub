using Cympatic.Extensions.Stub.Internal.Interfaces;
using Cympatic.Extensions.Stub.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Cympatic.Extensions.Stub.Internal;

internal static class SetupResponseEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapSetupResponse(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/setup", ([FromServices] IResponseSetupCollection collection)
            => collection.All());

        builder.MapGet("/setup/{id}", (Guid id, [FromServices] IResponseSetupCollection collection) 
            => collection.GetById(id) is ResponseSetup responseSetup
                ? Results.Ok(responseSetup)
                : Results.NotFound());

        builder.MapPost("/setup/response", (ResponseSetup responseSetup, [FromServices] IResponseSetupCollection collection) =>
        {
            collection.AddOrUpdate([ responseSetup ]);

            return Results.Created(new Uri("/setup").Append(responseSetup.Id.ToString("N")), responseSetup);
        });

        builder.MapPost("/setup/responses", (IEnumerable<ResponseSetup> responseSetups, [FromServices] IResponseSetupCollection collection) =>
        {
            collection.AddOrUpdate(responseSetups);

            return Results.NoContent();
        });

        builder.MapDelete("/setup/remove/{id}", (Guid id, [FromServices] IResponseSetupCollection collection) =>
        {
            if (collection.GetById(id) is ResponseSetup responseSetup)
            {
                collection.Remove(responseSetup);

                return Results.NoContent();
            }

            return Results.NotFound();
        });

        builder.MapDelete("/setup/clear", ([FromServices] IResponseSetupCollection collection) =>
        {
            collection.Clear();

            return Results.NoContent();
        });

        return builder;
    }
}
