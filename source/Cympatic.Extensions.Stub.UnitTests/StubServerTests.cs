using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.UnitTests.Attributes;
using Cympatic.Extensions.Stub.UnitTests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace Cympatic.Extensions.Stub.UnitTests;

public class StubServerTests : IDisposable
{
    private const string LocalHost = "127.0.0.1";

    private readonly StubServer _sut;
    private readonly FakeMessageHandler _fakeMessageHandler;

    public StubServerTests()
    {
        _fakeMessageHandler = new();
        _sut = new(() => new StubServerOptions { ConfigureHttpClientHandler = () => _fakeMessageHandler });
    }

    public void Dispose()
    {
        _sut?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task When_AddResponseSetupAsync_is_called_Then_this_redirected_to_the_method_SetupResponse()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/setup/response";

        // Act
        await _sut.AddResponseSetupAsync(new ResponseSetup());

        // Assert
        _fakeMessageHandler.CallCount("/setup/response").Should().Be(1);
        _fakeMessageHandler.Calls("/setup/response").Single().Method.Should().Be(HttpMethod.Post);
    }

    [Fact]
    public async Task When_AddResponsesSetupAsync_is_called_Then_this_redirected_to_the_method_SetupResponses()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/setup/responses";

        // Act
        await _sut.AddResponsesSetupAsync([new ResponseSetup()]);

        // Assert
        _fakeMessageHandler.CallCount("/setup/responses").Should().Be(1);
        _fakeMessageHandler.Calls("/setup/responses").Single().Method.Should().Be(HttpMethod.Post);
    }

    [Fact]
    public async Task When_ClearResponsesSetupAsync_is_called_Then_this_redirected_to_the_method_SetupClear()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/setup/clear";

        // Act
        await _sut.ClearResponsesSetupAsync();

        // Assert
        _fakeMessageHandler.CallCount("/setup/clear").Should().Be(1);
        _fakeMessageHandler.Calls("/setup/clear").Single().Method.Should().Be(HttpMethod.Delete);
    }

    [Fact]
    public async Task When_FindReceivedRequestsAsync_is_called_Then_this_redirected_to_the_method_ReceivedFind()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/received/find";

        // Act
        await _sut.FindReceivedRequestsAsync(new ReceivedRequestSearchParams(Guid.NewGuid().ToString("N")));

        // Assert
        _fakeMessageHandler.CallCount("/received/find").Should().Be(1);
        _fakeMessageHandler.Calls("/received/find").Single().Method.Should().Be(HttpMethod.Get);
    }

    [Fact]
    public async Task When_ClearReceivedRequestsAsync_is_called_Then_this_redirected_to_the_method_ReceivedClear()
    {
        // Arrange
        _fakeMessageHandler.ExpectedUrlPartial = "/received/clear";

        // Act
        await _sut.ClearReceivedRequestsAsync();

        // Assert
        _fakeMessageHandler.CallCount("/received/clear").Should().Be(1);
        _fakeMessageHandler.Calls("/received/clear").Single().Method.Should().Be(HttpMethod.Delete);
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


    [IgnoreOnLinuxFact]
    public void When_ServerCertificateSelector_returns_a_certificate_and_the_BaseAddress_is_called_Then_the_Scheme_is_Https()
    {
        // Arrange
        var sut = new StubServer(() => new StubServerOptions
        {
            ServerCertificateSelector = () =>
            {
                const string localHost = "localhost";

                using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine, OpenFlags.ReadOnly);
                store.Open(OpenFlags.ReadOnly);
                var certificate = store.Certificates
                    .Find(X509FindType.FindByIssuerName, localHost, false)
                    .Where(cert => cert.NotBefore <= DateTime.Now.Date && cert.NotAfter >= DateTime.Now.Date)
                    .OrderByDescending(cert => cert.NotAfter)
                    .FirstOrDefault();
                store.Close();

                return certificate;
            }
        });

        // Act & Assert
        sut.BaseAddress.Scheme.Should().Be(Uri.UriSchemeHttps);
    }

    [Fact]
    public void When_ServerCertificateSelector_returns_null_and_the_BaseAddress_is_called_Then_the_Scheme_is_Http()
    {
        // Arrange
        var sut = new StubServer(() => new StubServerOptions { ServerCertificateSelector = () => default });

        // Act & Assert
        sut.BaseAddress.Scheme.Should().Be(Uri.UriSchemeHttp);
    }

    [IgnoreOnLinuxFact]
    public void When_ServerCertificateSelector_returns_a_certificate_and_the_BaseAddressStub_is_called_Then_the_Scheme_is_Https()
    {
        // Arrange
        var sut = new StubServer(() => new StubServerOptions
        { 
            ServerCertificateSelector = () =>
            {
                const string localHost = "localhost";

                using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine, OpenFlags.ReadOnly);
                store.Open(OpenFlags.ReadOnly);
                var certificate = store.Certificates
                    .Find(X509FindType.FindByIssuerName, localHost, false)
                    .Where(cert => cert.NotBefore <= DateTime.Now.Date && cert.NotAfter >= DateTime.Now.Date)
                    .OrderByDescending(cert => cert.NotAfter)
                    .FirstOrDefault();
                store.Close();

                return certificate;
            }
        });

        // Act & Assert
        sut.BaseAddressStub.Scheme.Should().Be(Uri.UriSchemeHttps);
    }

    [Fact]
    public void When_ServerCertificateSelector_returns_null_and_the_BaseAddressStub_is_called_Then_the_Scheme_is_Http()
    {
        // Arrange
        var sut = new StubServer(() => new StubServerOptions { ServerCertificateSelector = () => default });

        // Act & Assert
        sut.BaseAddressStub.Scheme.Should().Be(Uri.UriSchemeHttp);
    }

    [Fact]
    public async Task When_ConfigureHttpClientHandler_returns_a_custom_Handler_Then_this_Handler_is_used_for_calls_to_the_StubServer()
    {
        // Arrange
        var fakeMessageHandler = new FakeMessageHandler
        {
            ExpectedUrlPartial = "/setup/response"
        };

        var sut = new StubServer(() => new StubServerOptions { ConfigureHttpClientHandler = () => fakeMessageHandler });

        // Act
        await sut.AddResponseSetupAsync(new ResponseSetup());

        // Assert
        fakeMessageHandler.CallCount("/setup/response").Should().Be(1);
        fakeMessageHandler.Calls("/setup/response").Single().Method.Should().Be(HttpMethod.Post);
    }
}
