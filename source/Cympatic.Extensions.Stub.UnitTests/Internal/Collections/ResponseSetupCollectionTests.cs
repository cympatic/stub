using Cympatic.Extensions.Stub.Internal.Collections;
using Cympatic.Extensions.Stub.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Cympatic.Extensions.Stub.UnitTests.Internal.Collections;

public class ResponseSetupCollectionTests : IDisposable
{
    private const int NumberOfItems = 10;

    private readonly ResponseSetupCollection _sut = new();

    public void Dispose()
    {
        _sut?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void When_Add_is_called_with_a_null_Item_Then_an_ArgumentNullException_is_thrown()
    {
        // Arrange & Act
        var act = () => _sut.AddOrUpdate(default!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("items");
    }

    [Fact]
    public void When_AddOrUpdate_is_called_with_nonexisting_items_Then_all_items_are_added()
    {
        // Arrange
        var expected = new List<ResponseSetup>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            expected.Add(GenerateResponseSetup());
        }

        // Act
        _sut.AddOrUpdate(expected);

        // Assert
        _sut.Count.Should().Be(NumberOfItems);
        _sut.All().Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void When_AddOrUpdate_is_called_with_an_items_with_existing_values_in_the_list_Then_the_already_existing_item_is_replaced_with_the_new_Item()
    {
        // Arrange
        var random = new Random();
        var list = new List<ResponseSetup>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            list.Add(GenerateResponseSetup());
        }
        list.ForEach(_sut.Add);

        var expected = CloneResponseSetup(list[random.Next(NumberOfItems)]);

        // Act
        _sut.AddOrUpdate([expected]);

        // Assert
        _sut.Count.Should().Be(NumberOfItems);
        _sut.All().Should().Contain(expected);
    }

    [Fact]
    public void When_GetById_is_called_with_a_valid_id_Then_the_existing_item_is_returned()
    {
        // Arrange
        var random = new Random();
        var list = new List<ResponseSetup>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            list.Add(GenerateResponseSetup());
        }
        list.ForEach(_sut.Add);

        var expected = list[random.Next(NumberOfItems)];

        // Act
        var actual = _sut.GetById(expected.Id);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void When_GetById_is_called_with_an_invalid_id_Then_no_item_is_returned()
    {
        // Arrange
        var random = new Random();
        var list = new List<ResponseSetup>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            list.Add(GenerateResponseSetup());
        }
        list.ForEach(_sut.Add);

        // Act
        var actual = _sut.GetById(Guid.NewGuid());

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public void When_Find_is_called_Then_matching_items_are_returned()
    {
        // Arrange
        var random = new Random();
        var query = new Dictionary<string, string>
        {
            { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") },
            { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") }
        };
        var list = new List<ResponseSetup>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            var item = GenerateResponseSetup();
            item.Query = query;

            list.Add(item);
        }
        list.ForEach(_sut.Add);

        var expected = list.Where(_ => _.HttpMethods.Contains(HttpMethod.Post.ToString()));

        // Act
        var actual = _sut.Find(HttpMethod.Post.ToString(), "{*}", new QueryCollection(query.Select(key => key).ToDictionary(item => item.Key, _ => new StringValues("{*}"))));

        // Assert
        actual.Should().BeEquivalentTo(expected);
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
            Response = Guid.NewGuid(),
            DelayInMilliseconds = random.Next(1000),
            Path = Guid.NewGuid().ToString("N"),
            Query = query,
            Headers = headers
        };
    }

    private static ResponseSetup CloneResponseSetup(ResponseSetup responseSetup)
        => new()
        {
            HttpMethods = responseSetup.HttpMethods,
            ReturnStatusCode = responseSetup.ReturnStatusCode,
            Location = responseSetup.Location,
            Response = responseSetup.Response,
            DelayInMilliseconds = responseSetup.DelayInMilliseconds,
            Path = responseSetup.Path,
            Query = responseSetup.Query,
            Headers = responseSetup.Headers
        };
}
