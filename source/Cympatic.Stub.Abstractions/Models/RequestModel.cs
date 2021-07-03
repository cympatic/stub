namespace Cympatic.Stub.Abstractions.Models
{
    public class RequestModel : StubModel
    {
        public string HttpMethod { get; set; }

        public string Body { get; set; }

        public bool ResponseFound { get; set; }
    }
}
