using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services;
using Cympatic.Extensions.Stub.UnitTests.Fakes;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Stub.UnitTests.Internal.Services;

public class ReceivedRequestApiServiceTests : IDisposable
{
    private const int NumberOfItems = 10;

    private readonly FakeMessageHandler _fakeMessageHandler;
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

    public void Dispose()
    {
        _fakeMessageHandler?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task When_GetAllASync_is_succesfullyly_called_Then_all_items_are_returned_and_made_1_GET_call_to_the_service()
    {
        // Arrange
        var expected = new List<ReceivedRequest>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            expected.Add(GenerateReceivedRequest());
        }
        _fakeMessageHandler.ExpectedUrlPartial = "/received";
        _fakeMessageHandler.Response = expected;

        // Act
        var actual = await _sut.GetAllAsync();

        // Assert
        actual.Should().BeEquivalentTo(expected, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
        _fakeMessageHandler.CallCount("/received").Should().Be(1);
        _fakeMessageHandler.Calls("/received").Single().Method.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public async Task When_GetAllASync_is_unsuccesfully_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/received";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = async () => await _sut.GetAllAsync();

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount("/received").Should().Be(1);
        _fakeMessageHandler.Calls("/received").Single().Method.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public async Task When_FindAsync_is_succesfully_called_Then_all_matching_items_are_returned_and_made_1_GET_call_to_the_service()
    {
        // Arrange
        const string requestPath = "/received/find?path=path&query[0].key=key1&query[0].value=value1&query[1].key=key2&query[1].value=value2&httpmethods[0]=httpmethod1&httpmethods[1]=httpmethod2";

        var expected = new List<ReceivedRequest>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            expected.Add(GenerateReceivedRequest());
        }
        _fakeMessageHandler.ExpectedUrlPartial = "/received/find";
        _fakeMessageHandler.Response = expected;

        var searchParams = new ReceivedRequestSearchParams("path", new Dictionary<string, string>
            {
                { "key1", "value1"},
                { "key2", "value2"}
            },
            ["httpmethod1", "httpmethod2"]);

        // Act
        var actual = await _sut.FindAsync(searchParams);

        // Assert
        // Assert
        actual.Should().BeEquivalentTo(expected, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
        _fakeMessageHandler.CallCount("/received/find").Should().Be(1);
        _fakeMessageHandler.Calls("/received/find").Single().Method.Should().Be(HttpMethod.Get);
        _fakeMessageHandler.Calls("/received/find").Single().RequestUri!.PathAndQuery.Should().Be(requestPath);
    }

    [Fact]
    public async Task When_FindASync_is_unsuccesfully_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/received/find";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = async () => await _sut.FindAsync(new ReceivedRequestSearchParams(string.Empty, new Dictionary<string, string>(), []));

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount("/received/find").Should().Be(1);
        _fakeMessageHandler.Calls("/received/find").Single().Method.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public async Task When_RemoveAllAsync_is_succesfully_called_Then_1_DELETE_call_is_made_to_the_service()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/received/clear";

        // Act
        await _sut.RemoveAllAsync();

        // Assert
        _fakeMessageHandler.CallCount("/received/clear").Should().Be(1);
        _fakeMessageHandler.Calls("/received/clear").Single().Method.Should().Be(HttpMethod.Delete);
    }

    [Fact]
    public async Task When_RemoveAllAsync_is_unsuccesfully_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/received/clear";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = async () => await _sut.RemoveAllAsync();

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount("/received/clear").Should().Be(1);
        _fakeMessageHandler.Calls("/received/clear").Single().Method.Should().Be(HttpMethod.Delete);
    }

    private static ReceivedRequest GenerateReceivedRequest()
    {
        var query = new Dictionary<string, string>
        {
            { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
            { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") }
        };
        var headers = new Dictionary<string, IEnumerable<string?>>
        {
            { Guid.NewGuid().ToString("N"), [ Guid.NewGuid().ToString("N") ] },
            { Guid.NewGuid().ToString("N"), [ Guid.NewGuid().ToString("N") ] }
        };

        return new
        (
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid().ToString("N"),
            query,
            headers,
            Guid.NewGuid().ToString("N"),
            Random.Shared.Next(2) == 0
        );
    }
}
