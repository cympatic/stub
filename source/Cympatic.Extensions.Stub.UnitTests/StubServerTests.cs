using Cympatic.Extensions.Stub.Services;
using Cympatic.Extensions.Stub.UnitTests.Attributes;
using FluentAssertions;
using Microsoft.Extensions.Hosting;

namespace Cympatic.Extensions.Stub.UnitTests;

public class StubServerTests : IDisposable
{
    private const string LocalHost = "127.0.0.1";

    private readonly StubServer _sut = new();

    public void Dispose()
    {
        _sut?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void When_Generic_CreateApiService_is_called_with_class_SetupResponseApiService_Then_an_instance_is_returned()
    {
        // Arrange & Act
        var actual = _sut.CreateApiService<SetupResponseApiService>();

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<SetupResponseApiService>();
    }

    [Fact]
    public void When_Generic_CreateApiService_is_called_with_class_ReceivedRequestApiService_Then_an_instance_is_returned()
    {
        // Arrange & Act
        var actual = _sut.CreateApiService<ReceivedRequestApiService>();

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<ReceivedRequestApiService>();
    }

    [Fact]
    public void When_CreateApiService_is_called_with_class_SetupResponseApiService_Then_an_instance_is_returned()
    {
        // Arrange & Act
        var actual = _sut.CreateApiService(typeof(SetupResponseApiService));

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<SetupResponseApiService>();
    }

    [Fact]
    public void When_CreateApiService_is_called_with_class_ReceivedRequestApiService_Then_an_instance_is_returned()
    {
        // Arrange & Act
        var actual = _sut.CreateApiService(typeof(ReceivedRequestApiService));

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<ReceivedRequestApiService>();
    }

    [Fact]
    public void When_CreateApiService_is_called_with_class_not_derived_from_the_class_ApiService_Then_an_InvalidOperationException_is_thrown()
    {
        // Arrange & Act
        var act = () => _sut.CreateApiService(typeof(string));

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Type: {typeof(string).Name} doesn't derive from class 'ApiService'");
    }

    [Fact]
    public void When_Host_is_called_Then_an_Instance_of_IHost_is_returned()
    {
        // Arrange & Act
        var actual = _sut.Host;

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<IHost>();
    }

    [Fact]
    public void When_ResetHost_is_called_Then_the_Host_Instance_is_Reset_and_a_call_of_Host_creates_a_new_IHost_Instance()
    {
        // Arrange
        var oldHost = _sut.Host;

        // Act
        _sut.ResetHost();
        var actual = _sut.Host;

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<IHost>();
        actual.Should().NotBe(oldHost);
    }

    [Fact]
    public void When_BaseAddress_is_called_Then_the_Host_is_LocalHost()
    {
        // Arrange & Act & Assert
        _sut.BaseAddress.Host.Should().Be(LocalHost);
    }

    [IgnoreOnLinuxFact]
    public void When_UseSsl_is_True_and_the_BaseAddress_is_called_Then_the_Scheme_is_Https()
    {
        // Arrange
        var sut = new StubServer(true);

        // Act & Assert
        sut.BaseAddress.Scheme.Should().Be(Uri.UriSchemeHttps);
    }

    [Fact]
    public void When_UseSsl_is_False_and_the_BaseAddress_is_called_Then_the_Scheme_is_Http()
    {
        // Arrange
        var sut = new StubServer(false);

        // Act & Assert
        sut.BaseAddress.Scheme.Should().Be(Uri.UriSchemeHttp);
    }

    [Fact]
    public void When_BaseAddressStub_is_called_Then_the_Host_is_LocalHost()
    {
        // Arrange & Act & Assert
        _sut.BaseAddressStub.Host.Should().Be(LocalHost);
    }

    [IgnoreOnLinuxFact]
    public void When_UseSsl_is_True_and_the_BaseAddressStub_is_called_Then_the_Scheme_is_Https()
    {
        // Arrange
        var sut = new StubServer(true);

        // Act & Assert
        sut.BaseAddressStub.Scheme.Should().Be(Uri.UriSchemeHttps);
    }

    [Fact]
    public void When_UseSsl_is_False_and_the_BaseAddressStub_is_called_Then_the_Scheme_is_Http()
    {
        // Arrange
        var sut = new StubServer(false);

        // Act & Assert
        sut.BaseAddressStub.Scheme.Should().Be(Uri.UriSchemeHttp);
    }

    [Fact]
    public void When_BaseAddressStub_is_called_Then_the_last_segment_is_Stub()
    {
        // Arrange & Act & Assert
        _sut.BaseAddressStub.Segments.Last().Should().Be("stub");
    }
}
