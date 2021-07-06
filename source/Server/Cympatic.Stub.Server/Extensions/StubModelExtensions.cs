using Cympatic.Extensions.Http;
using Cympatic.Stub.Connectivity.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cympatic.Stub.Server.Extensions
{
    public static class StubModelExtensions
    {
        public static string GetCreatedLocation(this ResponseModel model, string scheme, HostString host)
        {
            if (host.HasValue && model.Location != null && !model.Location.IsAbsoluteUri)
            {
                var uriBuilder = host.Port.HasValue 
                    ? new UriBuilder(scheme, host.Host, host.Port.Value)
                    : new UriBuilder(scheme, host.Host);

                return uriBuilder.Uri.Append(model.Location.ToString()).ToString();
            }

            return model.Location?.ToString() ?? string.Empty;
        }

        public static bool IsMatching(this ResponseModel model, string httpMethod, string path, IDictionary<string, string> query)
        {
            return IsMatching(model.HttpMethods, httpMethod, path, model.Path, query, model.Query);
        }

        public static bool IsMatching(this RequestModel model, IList<string> httpMethods, string path, IDictionary<string, string> query)
        {
            return IsMatching(httpMethods, model.HttpMethod, model.Path, path, model.Query, query);
        }

        public static bool AreRequestParamsEqual(this ResponseModel model, IList<string> httpMethods, string path, IDictionary<string, string> query)
        {
            var modelHttpMethods = model.HttpMethods ?? new List<string>();
            httpMethods ??= new List<string>();

            return
                modelHttpMethods.All(value => httpMethods.Contains(value, StringComparer.OrdinalIgnoreCase)) &&
                ComparePath(model.Path, path) &&
                CompareQuery(model.Query, query);
        }

        private static bool IsMatching(IList<string> httpMethods, string httpMethod, string path, string modelPath, IDictionary<string, string> query, IDictionary<string, string> modelQuery)
        {
            httpMethods ??= new List<string>();
            return
                (!httpMethods.Any() ||
                 httpMethods.Any(value => value.Equals(httpMethod, StringComparison.OrdinalIgnoreCase))) &&
                ComparePath(modelPath, path) &&
                CompareQuery(modelQuery, query);
        }

        private static bool ComparePath(string current, string other)
        {
            var currentPath = current.Split("/", StringSplitOptions.RemoveEmptyEntries).ToList();
            var otherPath = other.Split("/", StringSplitOptions.RemoveEmptyEntries).ToList();

            if (currentPath.Count != otherPath.Count)
            {
                return false;
            }

            for (int i = 0; i < currentPath.Count; i++)
            {
                if (!(currentPath[i].Equals(otherPath[i], StringComparison.OrdinalIgnoreCase) ||
                      currentPath[i].StartsWith("{*") && currentPath[i].EndsWith("}") ||
                      otherPath[i].StartsWith("{*") && otherPath[i].EndsWith("}")))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CompareQuery(IDictionary<string, string> current, IDictionary<string, string> other)
        {
            var currentQuery = new Dictionary<string, string>(current ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
            var otherQuery = new Dictionary<string, string>(other ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);

            return currentQuery.Count == otherQuery.Count &&
                currentQuery.Keys.All(key =>
                    otherQuery.ContainsKey(key) &&
                    (currentQuery[key].Equals(otherQuery[key], StringComparison.OrdinalIgnoreCase) ||
                     currentQuery[key].StartsWith("{*") && currentQuery[key].EndsWith("}")));
        }
    }
}
