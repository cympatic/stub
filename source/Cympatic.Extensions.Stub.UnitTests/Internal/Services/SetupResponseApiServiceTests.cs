using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services;
using Cympatic.Extensions.Stub.UnitTests.Fakes;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Stub.UnitTests.Internal.Services;

public class SetupResponseApiServiceTests : IDisposable
{
    private const int NumberOfItems = 10;

    private readonly FakeMessageHandler _fakeMessageHandler;
    private readonly SetupResponseApiService _sut;

    public SetupResponseApiServiceTests()
    {
        _fakeMessageHandler = new FakeMessageHandler();
        var httpClient = new HttpClient(_fakeMessageHandler)
        {
            BaseAddress = new Uri("http://fake.cympatic.com")
        };
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _sut = new SetupResponseApiService(httpClient);
    }

    public void Dispose()
    {
        _fakeMessageHandler?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task When_GetAllASync_is_succesfullyly_called_Then_all_items_are_returned_and_made_1_GET_call_to_the_service()
    {
        static IEnumerable<ResponseSetup> GetItems()
        {
            for (var i = 0; i < NumberOfItems; i++)
            {
                yield return GenerateResponseSetup();
            }
        }

        // Arrange
        var expected = GetItems().ToList();
        _fakeMessageHandler.ExpectedUrlPartial = "/setup";
        _fakeMessageHandler.Response = expected;

        // Act
        var actual = await _sut.GetAllAsync();

        // Assert
        actual.Should().BeEquivalentTo(expected, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
        _fakeMessageHandler.CallCount("/setup").Should().Be(1);
        _fakeMessageHandler.Calls("/setup").Single().Method.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public async Task When_GetAllASync_is_unsuccesfully_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/setup";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = async () => await _sut.GetAllAsync();

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount("/setup").Should().Be(1);
        _fakeMessageHandler.Calls("/setup").Single().Method.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public async Task When_GetByIdAsync_is_succesfully_called_Then_1_item_is_returned_and_made_1_GET_call_to_the_service()
    {
        // Arrange
        var expected = GenerateResponseSetup();
        _fakeMessageHandler.ExpectedUrlPartial = $@"/setup/{expected.Id:N}";
        _fakeMessageHandler.Response = expected;

        // Act
        var actual = await _sut.GetByIdAsync(expected.Id);

        // Assert
        actual.Should().BeEquivalentTo(expected, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
        _fakeMessageHandler.CallCount($@"/setup/{expected.Id:N}").Should().Be(1);
        _fakeMessageHandler.Calls($@"/setup/{expected.Id:N}").Single().Method.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public async Task When_GetByIdAsync_is_unsuccesfully_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        var id = Guid.NewGuid();
        _fakeMessageHandler.ExpectedUrlPartial = $@"/setup/{id:N}";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = async () => await _sut.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount($@"/setup/{id:N}").Should().Be(1);
        _fakeMessageHandler.Calls($@"/setup/{id:N}").Single().Method.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public async Task When_AddAsync_with_1_item_is_succesfully_called_Then_1_item_is_returned_and_made_1_POST_call_to_the_service()
    {
        // Arrange
        var expected = GenerateResponseSetup();
        _fakeMessageHandler.ExpectedUrlPartial = "/setup/response";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.Created;
        _fakeMessageHandler.Response = expected;

        // Act
        var actual = await _sut.AddAsync(expected);

        // Assert
        actual.Should().BeEquivalentTo(expected, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
        _fakeMessageHandler.CallCount("/setup/response").Should().Be(1);
        _fakeMessageHandler.Calls("/setup/response").Single().Method.Should().Be(HttpMethod.Post);
    }

    [Fact]
    public async Task When_AddAsync_with_1_item_is_unsuccesfully_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/setup/response";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = async () => await _sut.AddAsync(GenerateResponseSetup());

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount("/setup/response").Should().Be(1);
        _fakeMessageHandler.Calls("/setup/response").Single().Method.Should().Be(HttpMethod.Post);
    }

    [Fact]
    public async Task When_AddAsync_with_multiple_items_is_succesfully_called_Then_1_POST_call_is_made_to_the_service()
    {
        static IEnumerable<ResponseSetup> GetItems()
        {
            for (var i = 0; i < NumberOfItems; i++)
            {
                yield return GenerateResponseSetup();
            }
        }

        // Arrange
        var expected = GetItems().ToList();
        _fakeMessageHandler.ExpectedUrlPartial = "/setup/responses";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.Created;
        _fakeMessageHandler.Response = expected;

        // Act
        await _sut.AddAsync(expected);

        // Assert
        _fakeMessageHandler.CallCount("/setup/responses").Should().Be(1);
        _fakeMessageHandler.Calls("/setup/responses").Single().Method.Should().Be(HttpMethod.Post);
    }

    [Fact]
    public async Task When_AddAsync_with_multiple_items_is_unsuccesfully_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/setup/responses";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = async () => await _sut.AddAsync([]);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount("/setup/responses").Should().Be(1);
        _fakeMessageHandler.Calls("/setup/responses").Single().Method.Should().Be(HttpMethod.Post);
    }

    [Fact]
    public async Task When_RemoveAsync_with_an_item_is_succesfully_called_Then_1_DELETE_call_is_made_to_the_service()
    {
        // Arrange
        var expected = GenerateResponseSetup();
        _fakeMessageHandler.ExpectedUrlPartial = $@"/setup/remove/{expected.Id:N}";

        // Act
        await _sut.RemoveAsync(expected);

        // Assert
        _fakeMessageHandler.CallCount($@"/setup/remove/{expected.Id:N}").Should().Be(1);
        _fakeMessageHandler.Calls($@"/setup/remove/{expected.Id:N}").Single().Method.Should().Be(HttpMethod.Delete);
    }

    [Fact]
    public async Task When_RemoveAsync_with_an_item_is_unsuccesfully_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        var expected = GenerateResponseSetup();
        _fakeMessageHandler.ExpectedUrlPartial = $@"/setup/remove/{expected.Id:N}";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = async () => await _sut.RemoveAsync(expected.Id);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount($@"/setup/remove/{expected.Id:N}").Should().Be(1);
        _fakeMessageHandler.Calls($@"/setup/remove/{expected.Id:N}").Single().Method.Should().Be(HttpMethod.Delete);
    }

    [Fact]
    public async Task When_RemoveAsync_with_an_identifier_is_succesfully_called_Then_1_DELETE_call_is_made_to_the_service()
    {
        // Arrange
        var expected = Guid.NewGuid();
        _fakeMessageHandler.ExpectedUrlPartial = $@"/setup/remove/{expected:N}";

        // Act
        await _sut.RemoveAsync(expected);

        // Assert
        _fakeMessageHandler.CallCount($@"/setup/remove/{expected:N}").Should().Be(1);
        _fakeMessageHandler.Calls($@"/setup/remove/{expected:N}").Single().Method.Should().Be(HttpMethod.Delete);
    }

    [Fact]
    public async Task When_RemoveAsync_with_an_identifier_is_unsuccesfully_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        var expected = Guid.NewGuid();
        _fakeMessageHandler.ExpectedUrlPartial = $@"/setup/remove/{expected:N}";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = async () => await _sut.RemoveAsync(expected);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount($@"/setup/remove/{expected:N}").Should().Be(1);
        _fakeMessageHandler.Calls($@"/setup/remove/{expected:N}").Single().Method.Should().Be(HttpMethod.Delete);
    }

    [Fact]
    public async Task When_RemoveAllAsync_is_succesfully_called_Then_1_DELETE_call_is_made_to_the_service()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/setup/clear";

        // Act
        await _sut.RemoveAllAsync();

        // Assert
        _fakeMessageHandler.CallCount("/setup/clear").Should().Be(1);
        _fakeMessageHandler.Calls("/setup/clear").Single().Method.Should().Be(HttpMethod.Delete);
    }

    [Fact]
    public async Task When_RemoveAllAsync_is_unsuccesfully_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/setup/clear";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = async () => await _sut.RemoveAllAsync();

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount("/setup/clear").Should().Be(1);
        _fakeMessageHandler.Calls("/setup/clear").Single().Method.Should().Be(HttpMethod.Delete);
    }

    private static ResponseSetup GenerateResponseSetup()
    {
        var httpStatusCodes = Enum.GetValues(typeof(HttpStatusCode));
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
        string[] httpMethodNames =
        [
            "Connect",
            "Delete",
            "Get",
            "Head",
            "Options",
            "Patch",
            "Post",
            "Put",
            "Trace"
        ];
        var httpMethods = new List<string>();
        for (var i = 0; i < 3; i++)
        {
            httpMethods.Add(httpMethodNames[Random.Shared.Next(httpMethodNames.Length)]);
        }

        return new()
        {
            HttpMethods = httpMethods,
            ReturnStatusCode = (HttpStatusCode)httpStatusCodes.GetValue(Random.Shared.Next(httpStatusCodes.Length))!,
            Location = new Uri(Guid.NewGuid().ToString("N"), UriKind.Relative),
            DelayInMilliseconds = Random.Shared.Next(1000),
            Path = Guid.NewGuid().ToString("N"),
            Query = query,
            Headers = headers
        };
    }
}
