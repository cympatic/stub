namespace Cympatic.Extensions.Stub.Internal.Utilities;

internal class SearchableStubbedHttpItemUtility
{
    public static bool IsMatching(IList<string> httpMethods, string httpMethod, string path, string modelPath, IDictionary<string, string> query, IDictionary<string, string> modelQuery)
    {
        httpMethods ??= [];

        return
            (!httpMethods.Any() ||
             httpMethods.Any(value => value.Equals(httpMethod, StringComparison.OrdinalIgnoreCase))) &&
            ComparePath(modelPath, path) &&
            CompareQuery(modelQuery, query);
    }

    public static bool ComparePath(string current, string other)
    {
        var currentPath = current?.Split("/", StringSplitOptions.RemoveEmptyEntries).ToList() ?? [];
        var otherPath = other?.Split("/", StringSplitOptions.RemoveEmptyEntries).ToList() ?? [];

        if (currentPath.Count != otherPath.Count)
        {
            return false;
        }

        for (int i = 0; i < currentPath.Count; i++)
        {
            if (currentPath[i].Equals(otherPath[i], StringComparison.OrdinalIgnoreCase) ||
                currentPath[i].StartsWith("{*") && currentPath[i].EndsWith("*}"))
            {
                continue;
            }
            return false;
        }

        return true;
    }

    public static bool CompareQuery(IDictionary<string, string> current, IDictionary<string, string> other)
    {
        var currentQuery = new Dictionary<string, string>(current ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
        var otherQuery = new Dictionary<string, string>(other ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);

        return currentQuery.Count == otherQuery.Count &&
            currentQuery.Keys.All(key =>
                otherQuery.ContainsKey(key) &&
                (currentQuery[key].Equals(otherQuery[key], StringComparison.OrdinalIgnoreCase) ||
                 currentQuery[key].StartsWith("{*") && currentQuery[key].EndsWith("*}")));
    }
}
