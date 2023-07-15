using Cympatic.Stub.Connectivity.Extensions;
using FluentAssertions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace Cympatic.Stub.Connectivity.UnitTests.Extensions;

public class HttpHeadersExtensionsTests
{
    private const string IdentifierHeaderName = "StubIdentifierValue";
    private readonly HttpRequestMessage _sut;
    private readonly string _userId;

    public HttpHeadersExtensionsTests()
    {
        _sut = new HttpRequestMessage();
        _sut.Headers.Add("test1", "value1_1");
        _sut.Headers.Add("test2", "value2_1");

        _userId = Guid.NewGuid().ToString("N");
    }

    [Fact]
    public void Given_a_HttpRequestMessage_When_added_all_required_headers_Then_HasValidHeaders_should_return_True()
    {
        // Arrange
        _sut.Headers
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _sut.Headers.Add(IdentifierHeaderName, _userId);

        // Act
        var actual = _sut.Headers.HasValidHeaders(IdentifierHeaderName);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void Given_a_HttpRequestMessage_When_added_non_of_the_required_headers_Then_HasValidHeaders_should_return_False()
    {
        // Arrange & Act
        var actual = _sut.Headers.HasValidHeaders(IdentifierHeaderName);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Given_a_HttpRequestMessage_When_added_only_the_Accept_header_Then_HasValidHeaders_should_return_True()
    {
        // Arrange
        _sut.Headers
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Act
        var actual = _sut.Headers.HasValidHeaders(IdentifierHeaderName);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void Given_a_HttpRequestMessage_When_added_only_the_IdentifierHeaderName_header_Then_HasValidHeaders_should_return_False()
    {
        // Arrange
        _sut.Headers.Add(IdentifierHeaderName, _userId);

        // Act
        var actual = _sut.Headers.HasValidHeaders(IdentifierHeaderName);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Given_a_HttpRequestMessage_When_added_only_non_relevant_headers_Then_HasValidHeaders_should_return_False()
    {
        // Arrange
        _sut.Headers.Add("test3", "value4_1");
        _sut.Headers.Add("test4", "value4_1");

        // Act
        var actual = _sut.Headers.HasValidHeaders(IdentifierHeaderName);

        // Assert
        actual.Should().BeFalse();
    }
}
