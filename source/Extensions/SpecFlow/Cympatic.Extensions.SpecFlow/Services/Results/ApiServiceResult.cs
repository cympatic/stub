using Cympatic.Extensions.Http;
using Cympatic.Extensions.SpecFlow.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Extensions.SpecFlow.Services.Results
{
    public class ApiServiceResult : IApiServiceResult
    {
        public HttpStatusCode StatusCode { get; private set; }

        public HttpHeaders Headers { get; private set; }

        public Uri Location { get; private set; }

        public string Content { get; private set; }

        public bool IsSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode <= 299;

        public virtual async Task InitializeAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            StatusCode = response.StatusCode;
            Headers = response.Headers;
            Location = response.Headers.Location;
            Content = string.Empty;

            if (response.Content is object)
            {
                Headers.AddRange(response.Content.Headers);

                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream, cancellationToken);
                if (memoryStream.CanSeek)
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
                using StreamReader reader = new(memoryStream);
                Content = await reader.ReadToEndAsync();
            }
        }
    }
}
