using System.Collections.Specialized;
using System.Web;

namespace Cympatic.Extensions.Stub;

public static class UriExensions
{
    private const string DummyAbsoluteUri = "http://localhost";

    public static Uri Append(this Uri uri, params string[] paths)
    {
        var absoluteUri = uri.IsAbsoluteUri
            ? uri
            : new Uri(new Uri(DummyAbsoluteUri), uri);

        var resultUri = new Uri(paths.Aggregate(absoluteUri.AbsoluteUri, (current, path) => $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));
        if (uri.IsAbsoluteUri)
        {
            return resultUri;
        }

        var uriBuilder = new UriBuilder(resultUri);

        return new Uri(uriBuilder.Path, UriKind.Relative);
    }

    public static Uri WithParameter(this Uri uri, string paramName, string paramValue) 
        => uri.WithParameters(new NameValueCollection { { paramName, paramValue } });

    public static Uri WithParameters(this Uri uri, IDictionary<string, string> queryParams)
    {
        var parameters = new NameValueCollection();
        foreach (var (key, value) in queryParams)
        {
            parameters.Add(key, value);
        }

        return uri.WithParameters(parameters);
    }

    public static Uri WithParameters(this Uri uri, NameValueCollection queryParams)
    {
        ArgumentNullException.ThrowIfNull(queryParams);

        var absoluteUri = uri.IsAbsoluteUri ? uri : new Uri(new Uri(DummyAbsoluteUri), uri);

        var uriBuilder = new UriBuilder(absoluteUri);

        var collection = HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var key in queryParams.Cast<string>().Where(key => !string.IsNullOrEmpty(queryParams[key])))
        {
            collection[key] = queryParams[key];
        }
        uriBuilder.Query = collection.ToString();

        if (uri.IsAbsoluteUri)
        {
            return uriBuilder.Uri;
        }

        return new Uri(uriBuilder.Path + uriBuilder.Query, UriKind.Relative);
    }
}
