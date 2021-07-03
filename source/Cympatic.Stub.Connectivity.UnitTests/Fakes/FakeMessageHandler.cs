using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Stub.Connectivity.UnitTests.Fakes
{
    public class FakeMessageHandler : HttpMessageHandler
    {
        private readonly object _expectedReturnValue;
        private readonly string _expectedUrlPartial;
        private readonly HttpStatusCode _expectedStatusCode;
        private readonly IList<HttpRequestMessage> _calls = new List<HttpRequestMessage>();

        public FakeMessageHandler()
            : this(HttpStatusCode.OK, null, null)
        {
        }

        public FakeMessageHandler(object expectedReturnValue)
            : this(HttpStatusCode.OK, expectedReturnValue, null)
        {
        }
        public FakeMessageHandler(HttpStatusCode expectedStatusCode, string expectedUrlPartial)
            : this(expectedStatusCode, null, expectedUrlPartial)
        {
        }

        public FakeMessageHandler(object expectedReturnValue, string expectedUrlPartial)
            : this(HttpStatusCode.OK, expectedReturnValue, expectedUrlPartial)
        {
        }

        public FakeMessageHandler(HttpStatusCode expectedStatusCode, object expectedReturnValue, string expectedUrlPartial)
        {
            _expectedStatusCode = expectedStatusCode;
            _expectedReturnValue = expectedReturnValue;
            _expectedUrlPartial = expectedUrlPartial;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _calls.Add(request);

            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(_expectedUrlPartial) || request.RequestUri.AbsoluteUri.Contains(_expectedUrlPartial))
            {
                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = _expectedStatusCode,
                    Content = GetContent()
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        }

        public int CallCount(string uri) => _calls.Count(req => req.RequestUri.AbsoluteUri.ToLowerInvariant().Contains(uri.ToLowerInvariant()));

        private StringContent GetContent()
        {
            if (_expectedReturnValue == null)
            {
                return null;
            }

            var returnValue = _expectedReturnValue is string value
                ? value
                : JsonSerializer.Serialize(_expectedReturnValue);
            return new StringContent(returnValue);
        }
    }
}
