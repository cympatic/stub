using Cympatic.Extensions.Stub.Internal.Interfaces;
using Cympatic.Extensions.Stub.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Cympatic.Extensions.Stub.Internal;

internal static class ReceivedRequestEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapReceivedRequest(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/received", ([FromServices] IReceivedRequestCollection collection)
            => collection.All());

        builder.MapGet("/received/find", (ReceivedRequestSearchParams searchParams, [FromServices] IReceivedRequestCollection collection)
            => collection.Find(searchParams)); 

        builder.MapDelete("/received/clear", ([FromServices] IReceivedRequestCollection collection) =>
        {
            collection.Clear();

            return Results.NoContent();
        });

        return builder;
    }
}
