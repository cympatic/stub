using System.Collections.Generic;

namespace Cympatic.Stub.Abstractions.Models
{
    public class RequestSearchModel
    {
        public string Path { get; set; }

        public IDictionary<string, string> Query { get; set; }

        public IList<string> HttpMethods { get; set; }

        public RequestSearchModel()
        {
            Query = new Dictionary<string, string>();
            HttpMethods = new List<string>();
        }
    }
}
