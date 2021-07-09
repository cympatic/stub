using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Extensions.SpecFlow.Interfaces
{
    public interface IApiServiceResult
    {
        HttpStatusCode StatusCode { get; }

        HttpHeaders Headers { get; }

        Uri Location { get; }

        string Content { get; }

        bool IsSuccessStatusCode { get; }

        Task InitializeAsync(HttpResponseMessage response, CancellationToken cancellationToken = default);
    }
}
