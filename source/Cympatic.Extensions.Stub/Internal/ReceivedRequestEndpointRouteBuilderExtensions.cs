using Cympatic.Extensions.Stub.Internal.Collections;
using Cympatic.Extensions.Stub.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cympatic.Extensions.Stub.Internal;

internal static class ReceivedRequestEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapReceivedRequest(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/received", (ReceivedRequestCollection collection)
            => collection.All());

        builder.MapGet("/received/find", (ReceivedRequestSearchParams searchParams, ReceivedRequestCollection collection)
            => collection.Find(searchParams));

        builder.MapDelete("/received/clear", (ReceivedRequestCollection collection) =>
        {
            collection.Clear();

            return Results.NoContent();
        });

        return builder;
    }
}
