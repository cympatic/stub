using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services;
using Cympatic.Extensions.Stub.UnitTests.Fakes;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;

namespace Cympatic.Extensions.Stub.UnitTests.Services;

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

    [Fact]
    public async Task When_GetAllASync_is_succesful_called_Then_all_items_are_returned_and_made_1_call_to_the_service()
    {
        // Arrange
        var expected = new List<ResponseSetup>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            expected.Add(GenerateResponseSetup());
        }
        _fakeMessageHandler.ExpectedUrlPartial = "/setup";
        _fakeMessageHandler.Response = expected;

        // Act
        var actual = await _sut.GetAllAsync();

        // Assert
        actual.Should().BeEquivalentTo(expected, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
        _fakeMessageHandler.CallCount("/setup").Should().Be(1);
    }

    [Fact]
    public async Task When_GetAllASync_is_unsuccesful_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/setup";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = () => _sut.GetAllAsync();

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount("/setup").Should().Be(1);
    }

    [Fact]
    public async Task When_GetByIdAsync_is_succesful_called_Then_1_item_is_returned_and_made_1_call_to_the_service()
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
    }

    [Fact]
    public async Task When_GetByIdAsync_is_unsuccesful_called_Then_HttpRequestException_Thrown()
    {
        // Arrange
        var id = Guid.NewGuid();
        _fakeMessageHandler.ExpectedUrlPartial = $@"/setup/{id:N}";
        _fakeMessageHandler.ResponseStatusCode = HttpStatusCode.BadRequest;

        // Act
        var act = () => _sut.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
        _fakeMessageHandler.CallCount($@"/setup/{id:N}").Should().Be(1);
    }

    public void Dispose()
    {
        _fakeMessageHandler?.Dispose();
        GC.SuppressFinalize(this);
    }

    private static ResponseSetup GenerateResponseSetup()
    {
        var random = new Random();
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
            httpMethods.Add(httpMethodNames[random.Next(httpMethodNames.Length)]);
        }

        return new()
        {
            HttpMethods = httpMethods,
            ReturnStatusCode = (HttpStatusCode)httpStatusCodes.GetValue(random.Next(httpStatusCodes.Length))!,
            Location = new Uri(Guid.NewGuid().ToString("N"), UriKind.Relative),
            DelayInMilliseconds = random.Next(1000),
            Path = Guid.NewGuid().ToString("N"),
            Query = query,
            Headers = headers
        };
    }
}
