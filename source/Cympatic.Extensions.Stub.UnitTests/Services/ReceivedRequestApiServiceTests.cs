using Cympatic.Extensions.Stub.Services;
using Cympatic.Extensions.Stub.UnitTests.Fakes;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Stub.UnitTests.Services;

public class ReceivedRequestApiServiceTests
{
    private readonly HttpMessageHandler _fakeMessageHandler;
    private readonly ReceivedRequestApiService _sut;

    public ReceivedRequestApiServiceTests()
    {
        _fakeMessageHandler = new FakeMessageHandler();
        var httpClient = new HttpClient(_fakeMessageHandler)
        {
            BaseAddress = new Uri("http://fake.cympatic.com")
        };
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _sut = new ReceivedRequestApiService(httpClient);
    }
}
