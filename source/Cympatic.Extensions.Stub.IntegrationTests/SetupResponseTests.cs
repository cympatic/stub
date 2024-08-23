using Cympatic.Extensions.Stub.IntegrationTests.Fixtures;
using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services;
using FluentAssertions;

namespace Cympatic.Extensions.Stub.IntegrationTests;

public class SetupResponseTests : IClassFixture<StubServerFixture>
{
    private const int NumberOfItems = 10;

    private readonly StubServerFixture _fixture;
    private readonly SetupResponseApiService _sut;

    public SetupResponseTests(StubServerFixture fixture)
    {
        _fixture = fixture;
        _fixture.Clear();
        _sut = _fixture.SetupResponseApiService;
    }

    [Fact]
    public async Task When_added_1_SetupResponse_succesful_Then_Add_return_the_added_item()
    {
        // Arrange
        var addSetupResponse = _fixture.GenerateResponseSetup();

        // Act
        var addedSetupResponse = await _sut.AddAsync(addSetupResponse);
        var getSetupResponse = await _sut.GetByIdAsync(addedSetupResponse.Id);

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
    public async Task When_added_multiple_SetupResponses_succesful_Then_GetAllAsync_return_the_added_items()
    {
        // Arrange
        var expected = new List<ResponseSetup>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            expected.Add(_fixture.GenerateResponseSetup());
        }

        // Act
        await _sut.AddAsync(expected);
        var actual = await _sut.GetAllAsync();

        // Assert
        actual.Should().BeEquivalentTo(expected, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task When_a_ResponseSetup_is_removed_Then_GetAllAsync_is_no_longer_returning_the_item()
    {
        IEnumerable<ResponseSetup> GetItems()
        {
            for (var i = 0; i < NumberOfItems; i++)
            {
                yield return _fixture.GenerateResponseSetup();
            }
        }

        // Arrange
        await _sut.AddAsync(GetItems());
        var list = await _sut.GetAllAsync();
        var unexpected = list.ElementAt(Random.Shared.Next(list.Count()));

        // Act
        await _sut.RemoveAsync(unexpected);
        var actual = await _sut.GetAllAsync();

        // Assert
        actual.Count().Should().Be(9);
        actual.Should().NotContainEquivalentOf(unexpected);
    }

    [Fact]
    public void When_an_invalid_Identifier_is_used_to_Fetch_a_ResponseSetup_Then_a_HttpRequestExpection_is_thrown()
    {
        // Arrange & Act
        var act = async () => await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task When_all_ResponseSetups_are_cleared_Then_GetAllAsync_return_an_empty_Enumerable()
    {
        IEnumerable<ResponseSetup> GetItems()
        {
            for (var i = 0; i < NumberOfItems; i++)
            {
                yield return _fixture.GenerateResponseSetup();
            }
        }

        // Arrange
        await _sut.AddAsync(GetItems());

        // Act
        await _sut.RemoveAllAsync();
        var actual = await _sut.GetAllAsync();

        // Assert
        actual.Count().Should().Be(0);
    }
}
