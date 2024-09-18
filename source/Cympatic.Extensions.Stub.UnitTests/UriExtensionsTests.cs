using Cympatic.Extensions.Stub.Internal;
using FluentAssertions;
using System.Collections.Specialized;

namespace Cympatic.Extensions.Stub.UnitTests;

public class UriExtensionsTests
{
    [Fact]
    public void Given_an_Uri_When_a_segment_is_appended_Then_Uri_should_be_formatted_as_expected()
    {
        // Arrange
        var uri = new Uri("http://localhost/baseaddress");

        // Act
        uri = uri.Append("test");

        // Assert
        uri.ToString().Should().BeEquivalentTo("http://localhost/baseaddress/test");
    }

    [Fact]
    public void Given_an_Uri_When_multiple_segments_are_appended_Then_Uri_should_be_formatted_as_expected()
    {
        // Arrange
        var uri = new Uri("http://localhost/baseaddress");

        // Act
        uri = uri.Append("test1", "test2");

        // Assert
        uri.ToString().Should().BeEquivalentTo("http://localhost/baseaddress/test1/test2");
    }

    [Fact]
    public void Given_a_relative_Uri_When_query_string_params_are_added_Then_Uri_should_be_formatted_as_expected()
    {
        // Arrange
        var uri = new Uri("/baseaddress", UriKind.Relative);

        // Act
        uri = uri.WithParameter("test1", "test2");

        // Assert
        uri.ToString().Should().BeEquivalentTo("/baseaddress?test1=test2");
    }

    [Fact]
    public void Given_a_relative_Uri_When_namevalue_params_are_added_Then_Uri_should_be_formatted_as_expected()
    {
        // Arrange
        var uri = new Uri("/baseaddress", UriKind.Relative);
        var namedValueParam = new NameValueCollection { { "test1", "test2" } };

        // Act
        uri = uri.WithParameters(namedValueParam);

        // Assert
        uri.ToString().Should().BeEquivalentTo("/baseaddress?test1=test2");
    }

    [Fact]
    public void Given_an_Uri_When_empty_namevalue_params_are_added_Then_Uri_should_be_formatted_as_expected()
    {
        // Arrange
        var uri = new Uri("/baseaddress", UriKind.Relative);
        var namedValueParam = new NameValueCollection();

        // Act
        uri = uri.WithParameters(namedValueParam);

        // Assert
        uri.ToString().Should().BeEquivalentTo("/baseaddress");
    }

    [Fact]
    public void Given_an_absolute_Uri_When_a_queryparam_is_valid_Then_Uri_should_be_formatted_as_expected()
    {
        // Arrange
        var uri = new Uri("https://localhost/baseaddress", UriKind.Absolute);

        // Act
        uri = uri.WithParameter("test1", "test2");

        // Assert
        uri.ToString().Should().BeEquivalentTo("https://localhost/baseaddress?test1=test2");
    }

    [Fact]
    public void Given_an_absolute_Uri_When_all_queryparams_are_valid_in_the_NameValueCollection_Then_Uri_should_be_formatted_as_expected()
    {
        // Arrange
        var uri = new Uri("https://localhost/baseaddress", UriKind.Absolute);

        // Act
        uri = uri.WithParameters(new NameValueCollection
        {
            { "test1", "value1" },
            { "test2", "value2" },
            { "test3", "value3" },
        });

        // Assert
        uri.ToString().Should().BeEquivalentTo("https://localhost/baseaddress?test1=value1&test2=value2&test3=value3");
    }

    [Fact]
    public void Given_an_absolute_Uri_When_all_queryparams_are_valid_in_the_Dictionary_Then_Uri_should_be_formatted_as_expected()
    {
        // Arrange
        var uri = new Uri("https://localhost/baseaddress", UriKind.Absolute);

        // Act
        uri = uri.WithParameters(new Dictionary<string, string>
        {
            { "test1", "value1" },
            { "test2", "value2" },
            { "test3", "value3" },
        });

        // Assert
        uri.ToString().Should().BeEquivalentTo("https://localhost/baseaddress?test1=value1&test2=value2&test3=value3");
    }

    [Fact]
    public void Given_an_Uri_When_no_NameValueCollection_queryparams_Then_exception_should_be_thrown()
    {
        // Arrange
        var uri = new Uri("baseaddress", UriKind.Relative);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => uri.WithParameters((NameValueCollection)default!));
    }

    [Fact]
    public void Given_an_Uri_When_no_Dictionary_queryparams_Then_exception_should_be_thrown()
    {
        // Arrange
        var uri = new Uri("baseaddress", UriKind.Relative);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => uri.WithParameters((NameValueCollection)default!));
    }
}
