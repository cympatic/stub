using Cympatic.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

namespace Cympatic.Extensions.SpecFlow
{
    public class StubUrl : IEquatable<StubUrl>
    {
        public string Path { get; private set; }

        public IDictionary<string, string> QueryParams { get; private set; }

        public IEnumerable<string> HttpMethods { get; private set; }

        public HttpStatusCode ReturnHttpStatusCode { get; private set; }

        public StubUrl() 
            : this(string.Empty, new Dictionary<string, string>(), new List<string>(), HttpStatusCode.OK)
        { }

        public StubUrl(string path) 
            : this(path, new Dictionary<string, string>(), new List<string>(), HttpStatusCode.OK)
        { }

        public StubUrl(string path, IDictionary<string, string> queryParams)
            : this(path, queryParams, new List<string>(), HttpStatusCode.OK)
        { }

        public StubUrl(string path, IDictionary<string, string> queryParams, IList<string> httpMethods)
            : this(path, queryParams, httpMethods, HttpStatusCode.OK)
        { }

        public StubUrl(string path, IDictionary<string, string> queryParams, IList<string> httpMethods, HttpStatusCode returnHttpStatusCode)
        {
            Path = path;
            QueryParams = queryParams ?? new Dictionary<string, string>();
            HttpMethods = httpMethods ?? new List<string>();
            ReturnHttpStatusCode = returnHttpStatusCode;
        }

        public void SetPath(string path)
        {
            Path = path;
        }

        public void SetQueryParams(IDictionary<string, string> queryParams)
        {
            QueryParams = queryParams ?? new Dictionary<string, string>();
        }

        public void SetHttpMethods(IEnumerable<string> httpMethods)
        {
            HttpMethods = httpMethods ?? new List<string>();
        }

        public void SetReturnHttpStatusCode(HttpStatusCode returnHttpStatusCode)
        {
            ReturnHttpStatusCode = returnHttpStatusCode;
        }

        public Uri ToUri()
        {
            var uri = new Uri(Path, UriKind.Relative);

            var nameValueCollection = new NameValueCollection();
            foreach (var (key, value) in QueryParams)
            {
                nameValueCollection.Add(key, value);
            }

            return uri.WithParameters(nameValueCollection);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StubUrl);
        }

        public bool Equals(StubUrl other)
        {
            return other != null &&
                   Path == other.Path &&
                   EqualityComparer<IDictionary<string, string>>.Default.Equals(QueryParams, other.QueryParams) &&
                   EqualityComparer<IEnumerable<string>>.Default.Equals(HttpMethods, other.HttpMethods) &&
                   ReturnHttpStatusCode == other.ReturnHttpStatusCode;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Path, QueryParams, HttpMethods, ReturnHttpStatusCode);
        }
    }
}
