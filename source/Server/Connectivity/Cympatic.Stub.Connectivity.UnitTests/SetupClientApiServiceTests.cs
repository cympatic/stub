using Cympatic.Extensions.Http;
using Cympatic.Stub.Connectivity.Constants;
using Cympatic.Stub.Connectivity.Internal;
using Cympatic.Stub.Connectivity.UnitTests.Fakes;
using FluentAssertions;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cympatic.Stub.Connectivity.UnitTests
{
    public class SetupClientApiServiceTests
    {
        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_the_default_SetupAsync_Then_a_default_ClientStub_should_be_returned()
        {
            // Arrange
            var expected = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName, Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes);
            var uri = new Uri("add", UriKind.Relative)
                .WithParameters(new NameValueCollection {
                    { "identifierHeader", expected.IdentifierHeaderName },
                    { "responseTtlInMinutes", expected.ResponseTtlInMinutes.ToString() },
                    { "requestTtlInMinutes", expected.RequestTtlInMinutes.ToString() }
                });
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.Created, uri.ToString());
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act
            var actual = await sut.SetupAsync();

            // Assert
            actual.Should().BeEquivalentTo(expected, options => options.Excluding(model => model.Name));
            fakeMessageHandler.CallCount(uri.ToString()).Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_the_default_SetupAsync_with_Name_and_IdentifierHeaderName_Then_a_new_ClientStub_should_be_returned()
        {
            // Arrange
            var expected = new ClientStub("TestClientStub", "TestIdentifierHeaderName", Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes);
            var uri = new Uri("add", UriKind.Relative)
                .WithParameters(new NameValueCollection {
                    { "identifierHeader", expected.IdentifierHeaderName },
                    { "responseTtlInMinutes", expected.ResponseTtlInMinutes.ToString() },
                    { "requestTtlInMinutes", expected.RequestTtlInMinutes.ToString() }
                });
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.Created, uri.ToString());
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act
            var actual = await sut.SetupAsync("TestClientStub", "TestIdentifierHeaderName");

            // Assert
            actual.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount(uri.ToString()).Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_SetupAsync_with_a_new_ClientStub_Then_should_return_void()
        {
            // Arrange
            var expected = new ClientStub("TestClientStub", "TestIdentifierHeaderName", Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes);
            var uri = new Uri("add", UriKind.Relative)
                .WithParameters(new NameValueCollection {
                    { "identifierHeader", expected.IdentifierHeaderName },
                    { "responseTtlInMinutes", expected.ResponseTtlInMinutes.ToString() },
                    { "requestTtlInMinutes", expected.RequestTtlInMinutes.ToString() }
                });
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.Created, uri.ToString());
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act
            await sut.SetupAsync(expected);

            // Assert
            fakeMessageHandler.CallCount(uri.ToString()).Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_SetupAsync_with_null_Then_an_ArgumentNullException_should_be_thrown()
        {
            // Arrange
            var expected = new ClientStub("TestClientStub", "TestIdentifierHeaderName", Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes);
            var uri = new Uri("add", UriKind.Relative)
                .WithParameters(new NameValueCollection {
                    { "identifierHeader", expected.IdentifierHeaderName },
                    { "responseTtlInMinutes", expected.ResponseTtlInMinutes.ToString() },
                    { "requestTtlInMinutes", expected.RequestTtlInMinutes.ToString() }
                });
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.Created, uri.ToString());
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.SetupAsync(null));
            fakeMessageHandler.CallCount(uri.ToString()).Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_SetupAsync_with_a_new_ClientStub_and_return_HttpError_Then_an_HttpRequestException_should_be_thrown()
        {
            // Arrange
            var expected = new ClientStub("TestClientStub", "TestIdentifierHeaderName", Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes);
            var uri = new Uri("add", UriKind.Relative)
                .WithParameters(new NameValueCollection {
                    { "identifierHeader", expected.IdentifierHeaderName },
                    { "responseTtlInMinutes", expected.ResponseTtlInMinutes.ToString() },
                    { "requestTtlInMinutes", expected.RequestTtlInMinutes.ToString() }
                });
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.BadRequest, uri.ToString());
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => sut.SetupAsync(expected));
            fakeMessageHandler.CallCount(uri.ToString()).Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_the_default_RemoveAsync_Then_void_should_be_returned()
        {
            // Arrange
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.NoContent, "remove");
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act
            await sut.RemoveAsync();

            // Assert
            fakeMessageHandler.CallCount("remove").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_RemoveAsync_with_a_new_ClientStub_Then_should_return_void()
        {
            // Arrange
            var clientStub = new ClientStub("TestClientStub", "TestIdentifierHeaderName", Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.NoContent, "remove");
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act
            await sut.RemoveAsync(clientStub);

            // Assert
            fakeMessageHandler.CallCount("remove").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_RemoveAsync_with_null_Then_an_ArgumentNullException_should_be_thrown()
        {
            // Arrange
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.NoContent, "remove");
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.RemoveAsync(null));
            fakeMessageHandler.CallCount("remove").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_RemoveAsync_with_a_new_ClientStub_and_return_HttpError_Then_an_HttpRequestException_should_be_thrown()
        {
            // Arrange
            var clientStub = new ClientStub("TestClientStub", "TestIdentifierHeaderName", Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.BadRequest, "remove");
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => sut.RemoveAsync(clientStub));
            fakeMessageHandler.CallCount("remove").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_GetClientAsync_with_a_clientname_Then_should_return_a_IClientStub()
        {
            // Arrange
            var clientStub = new ClientStub("TestClientStub", "TestIdentifierHeaderName", Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, clientStub, "getclient");
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act
            var actual = await sut.GetClientAsync(clientStub.Name);

            // Assert
            actual.Should().BeEquivalentTo(clientStub);
            fakeMessageHandler.CallCount("getclient").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_GetClientAsync_with_an_invalid_clientname_Then_should_return_null()
        {
            // Arrange
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.BadRequest, null, "getclient");
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act
            var actual = await sut.GetClientAsync("fake");

            // Assert
            actual.Should().BeNull();
            fakeMessageHandler.CallCount("getclient").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupClientApiService_When_using_GetClientAsync_with_null_Then_an_ArgumentNullException_should_be_thrown()
        {
            // Arrange
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, "getclient");
            var sut = new SetupClientApiService(new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            });

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetClientAsync(null));
            fakeMessageHandler.CallCount("getclient").Should().Be(0);
        }
    }
}
