using Cympatic.Extensions.Stub.Internal.Collections;
using Cympatic.Extensions.Stub.Models;
using FluentAssertions;

namespace Cympatic.Extensions.Stub.UnitTests.Internal.Collections;

public class ReceivedRequestCollectionTests : IDisposable
{
    private const int NumberOfItems = 10;

    private readonly ReceivedRequestCollection _sut = new();

    public void Dispose()
    {
        _sut?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void When_Find_is_called_with_strict_SearchParams_Then_the_matching_Items_where_FoundMatchingResponse_is_True_are_return()
    {
        // Arrange
        var random = new Random();
        var list = new List<ReceivedRequest>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            list.Add(GenerateReceivedRequest());
        }
        list.ForEach(_sut.Add);

        var expected = list
            .Where(_ => _.FoundMatchingResponse)
            .ToList()[random.Next(list.Where(_ => _.FoundMatchingResponse).Count())];
        var searchParams = new ReceivedRequestSearchParams(expected.Path, expected.Query!, [expected.HttpMethod!]);

        // Act
        var actual = _sut.Find(searchParams);

        // Arrange
        actual.Should().BeEquivalentTo([expected]);
    }

    [Fact]
    public void When_Find_is_called_with_valid_SearchParams_Then_the_matching_Items_where_FoundMatchingResponse_is_True_are_return()
    {
        // Arrange
        var random = new Random();
        var query = new Dictionary<string, string>
        {
            { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
            { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") }
        };
        var list = new List<ReceivedRequest>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            var item = GenerateReceivedRequest(query);

            list.Add(item);
        }
        list.ForEach(_sut.Add);

        var expected = list
            .Where(_ => _.FoundMatchingResponse)
            .ToList();
        var searchParams = new ReceivedRequestSearchParams("{*}", query.Select(key => key).ToDictionary(item => item.Key, _ => "{*}"), []);

        // Act
        var actual = _sut.Find(searchParams);

        // Arrange
        actual.Should().BeEquivalentTo(expected);
    }

    private static ReceivedRequest GenerateReceivedRequest(Dictionary<string, string>? query = null)
    {
        var random = new Random();
        query ??= new Dictionary<string, string>
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
            random.Next(2) == 0
        );
    }
}
