﻿using Cympatic.Extensions.Stub.Internal;
using FluentAssertions;

namespace Cympatic.Extensions.Stub.UnitTests.Internal;

public class HttpHeadersExtensionsTests
{
    private readonly HttpRequestMessage _sut;

    public HttpHeadersExtensionsTests()
    {
        _sut = new HttpRequestMessage();
        _sut.Headers.Add("test1", "value1_1");
        _sut.Headers.Add("test2", "value2_1");
    }

    [Fact]
    public void When_AddRange_is_called_with_additional_headers_Then_the_headers_should_be_combined_in_the_existing_headers()
    {
        // Arrange & Act
        _sut.Headers.AddRange(new Dictionary<string, IEnumerable<string>>
        {
            { "test3", [ "value3_1" ] },
            { "test4", [ "value4_1" ] }
        });

        // Assert
        _sut.Headers
            .Should()
            .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
            {
                { "test1", [ "value1_1" ] },
                { "test2", [ "value2_1" ] },
                { "test3", [ "value3_1" ] },
                { "test4", [ "value4_1" ] }
            });
    }

    [Fact]
    public void When_AddRange_is_called_with_no_new_headers_Then_the_headers_should_be_as_is()
    {
        // Arrange & Act
        _sut.Headers.AddRange([]);

        // Assert
        _sut.Headers
            .Should()
            .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
            {
                { "test1", [ "value1_1" ] },
                { "test2", [ "value2_1" ] }
            });
    }

    [Fact]
    public void When_AddRange_is_called_with_null_value_Then_the_headers_should_be_as_is()
    {
        // Arrange & Act
        _sut.Headers.AddRange(null);

        // Assert
        _sut.Headers
            .Should()
            .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
            {
                { "test1", [ "value1_1" ] },
                { "test2", [ "value2_1" ] }
            });
    }

    [Fact]
    public void When_AddRange_is_called_with_similar_headers_and_different_values_Then_the_headers_should_be_combined_in_the_existing_headers()
    {
        // Arrange & Act
        _sut.Headers.AddRange(new Dictionary<string, IEnumerable<string>>
        {
            { "test2",  [ "value2_2" ] },
            { "test3",  [ "value3_1" ] }
        });

        // Assert
        _sut.Headers
            .Should()
            .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
            {
                { "test1", [ "value1_1" ] },
                { "test2", [ "value2_1", "value2_2" ] },
                { "test3", [ "value3_1" ] }
            });
    }

    [Fact]
    public void When_AddRange_is_called_with_similar_headers_and_similar_values_Then_the_headers_should_be_combined_in_the_existing_headers()
    {
        // Arrange & Act
        _sut.Headers.AddRange(new Dictionary<string, IEnumerable<string>>
        {
            { "test2", [ "value2_1" ] },
            { "test3", [ "value3_1" ] }
        });

        // Assert
        _sut.Headers
            .Should()
            .BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
            {
                { "test1", [ "value1_1" ] },
                { "test2", [ "value2_1" ] },
                { "test3", [ "value3_1" ] }
            });
    }
}
