using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Containers;
using Cympatic.Stub.Server.UnitTests.TestData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cympatic.Stub.Server.UnitTests.Containers;

public class RequestModelContainerTests
{
    private readonly RequestModelContainer _sut;
    private readonly string _identifierValue;

    public RequestModelContainerTests()
    {
        _sut = new RequestModelContainer(Mock.Of<ILogger<RequestModelContainer>>());
        _identifierValue = Guid.NewGuid().ToString("N");
    }

    [Fact]
    public void AddRequest()
    {
        var expected = RequestModelTestData.GetRequestModel();

        var actual = _sut.AddRequest(_identifierValue,
            expected.Path, expected.Query, expected.HttpMethod, expected.Headers, expected.Body, expected.ResponseFound);

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("segment1/{*wildcard}/segment3", "GET", 1)]
    [InlineData("segment1/wildcard/segment3", "", 2)]
    [InlineData("segment1/segment2/segment3", "", 0)]
    public void Find(string path, string httpMethod, int expectedCount)
    {
        var models = PrepareContainer(_identifierValue);
        var expected = expectedCount != 0
            ? models.Where(model => string.IsNullOrEmpty(httpMethod) || model.HttpMethod == httpMethod)
            : new List<RequestModel>();

        var httpMethods = !string.IsNullOrEmpty(httpMethod)
            ? new List<string> { httpMethod }
            : new List<string>();

        var actual = _sut.Find(_identifierValue,
            path,
            new Dictionary<string, string>
            {
                { "queryparam1", "1" }
            },
            httpMethods);

        actual.Should().HaveCount(expectedCount);
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Get_WithItems()
    {
        var expected = PrepareContainer(_identifierValue);

        var actual = _sut.Get(_identifierValue);

        actual.Should().HaveCount(expected.Count());
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Get_WithNoItems()
    {
        var actual = _sut.Get(_identifierValue);

        actual.Should().HaveCount(0);
    }

    [Fact]
    public void Remove()
    {
        PrepareContainer(_identifierValue);

        _sut.Remove(_identifierValue);
        var actual = _sut.Get(_identifierValue);

        actual.Should().HaveCount(0);
    }

    private IEnumerable<RequestModel> PrepareContainer(string identifierValue)
    {
        var models = RequestModelTestData.GetRequestModels();
        foreach (var model in models)
        {
            _sut.AddRequest(identifierValue, model.Path, model.Query, model.HttpMethod, model.Headers, model.Body, model.ResponseFound);
        }

        return models;
    }
}
