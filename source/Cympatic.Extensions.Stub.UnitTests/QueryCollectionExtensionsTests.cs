using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Cympatic.Extensions.Stub.UnitTests;

public class QueryCollectionExtensionsTests
{
    [Fact]
    public void Given_a_QueryCollection_When_ToDictionary_Then_the_Dictionary_should_items_correctly_formatted()
    {
        // Arrange
        var sut = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "test1", "value1" },
            { "test2", new [] { "value2", "value3" } }
        });

        // Act
        var actual = sut.ToDictionary();

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "test1", "value1" },
            { "test2", "value2,value3" }
        });
    }

    [Fact]
    public void Given_am_empty_QueryCollection_When_ToDictionary_Then_the_Dictionary_should_empty()
    {
        // Arrange
        var sut = new QueryCollection(new Dictionary<string, StringValues>());

        // Act
        var actual = sut.ToDictionary();

        // Assert
        actual.Should().BeEquivalentTo(new Dictionary<string, string>());
    }
}
