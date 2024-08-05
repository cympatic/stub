using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services;
using FluentAssertions;
using System.Net;

namespace Cympatic.Extensions.Stub.IntegrationTests;

public class SetupResponseTests : IDisposable
{
    private const int NumberOfItems = 10;

    private readonly StubServer _sut = new();

    public void Dispose()
    {
        _sut?.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task When_added_1_SetupResponse_succesful_Then_that_item_is_returned_and_can_be_fetched_on_its_Identifier()
    {
        // Arrange
        var setupReponseApiService = _sut.CreateApiService<SetupResponseApiService>();
        var addSetupResponse = GenerateResponseSetup();

        // Act
        var addedSetupResponse = await setupReponseApiService.AddAsync(addSetupResponse);
        var getSetupResponse = await setupReponseApiService.GetByIdAsync(addedSetupResponse.Id);

        // Assert
        addSetupResponse.Should().BeEquivalentTo(getSetupResponse, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
        addSetupResponse.Should().BeEquivalentTo(addedSetupResponse, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
        addedSetupResponse.Should().BeEquivalentTo(getSetupResponse);
    }

    [Fact]
    public async Task When_added_multiple_SetupResponses_succesful_Then_those_items_can_be_fetched_through_the_GetAllAsync()
    {
        // Arrange
        var setupReponseApiService = _sut.CreateApiService<SetupResponseApiService>();
        var expected = new List<ResponseSetup>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            expected.Add(GenerateResponseSetup());
        }

        // Act
        await setupReponseApiService.AddAsync(expected);
        var actual = await setupReponseApiService.GetAllAsync();

        // Assert
        actual.Should().BeEquivalentTo(expected, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task When_a_ResponseSetup_is_removed_Then_this_item_is_nolonger_available_through_GetAllAsync()
    {
        static IEnumerable<ResponseSetup> GetItems()
        {
            for (var i = 0; i < NumberOfItems; i++)
            {
                yield return GenerateResponseSetup();
            }
        }

        // Arrange
        var random = new Random();
        var setupReponseApiService = _sut.CreateApiService<SetupResponseApiService>();
        await setupReponseApiService.AddAsync(GetItems());
        var list = await setupReponseApiService.GetAllAsync();
        var unexpected = list.ElementAt(random.Next(list.Count()));

        // Act
        await setupReponseApiService.RemoveAsync(unexpected);
        var actual = await setupReponseApiService.GetAllAsync();

        // Assert
        actual.Count().Should().Be(9);
        actual.Should().NotContainEquivalentOf(unexpected);
    }

    [Fact]
    public void When_an_invalid_Identifier_is_used_to_Fetch_a_ResponseSetup_Then_a_HttpRequestExpection_is_thrown()
    {
        // Arrange
        var setupReponseApiService = _sut.CreateApiService<SetupResponseApiService>();

        // Act
        var act = async () => await setupReponseApiService.GetByIdAsync(Guid.NewGuid());

        // Assert
        act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task When_all_ResponseSetups_are_cleared_Then_GetAllAsync_return_an_empty_list()
    {
        static IEnumerable<ResponseSetup> GetItems()
        {
            for (var i = 0; i < NumberOfItems; i++)
            {
                yield return GenerateResponseSetup();
            }
        }

        // Arrange
        var setupReponseApiService = _sut.CreateApiService<SetupResponseApiService>();
        await setupReponseApiService.AddAsync(GetItems());

        // Act
        await setupReponseApiService.RemoveAllAsync();
        var actual = await setupReponseApiService.GetAllAsync();

        // Assert
        actual.Count().Should().Be(0);
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
