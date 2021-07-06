using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace Cympatic.Stub.Server
{
    public class RouteTransformer : DynamicRouteValueTransformer
    {
        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (values.TryGetValue("controller", out var value) &&
                value.ToString().ToLowerInvariant() == "stub" &&
                values.ContainsKey("slug"))
            {
                values.Add("action", "call");
                return new ValueTask<RouteValueDictionary>(values);
            }

            if (values.TryGetValue("slug", out var slug) && slug is string action)
            {
                values.Remove("slug");
                values.Add("action", action);
            }
            return new ValueTask<RouteValueDictionary>(values);
        }
    }
}
