using Cympatic.Stub.Abstractions.Constants;
using Cympatic.Stub.Abstractions.Models;
using Cympatic.Stub.Connectivity.Internal;
using Cympatic.Stub.Connectivity.UnitTests.Fakes;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Cympatic.Stub.Connectivity.UnitTests
{
    public class SetupResponseApiServiceTests
    {
        [Fact]
        public void Given_a_SetupResponseApiService_When_using_SetIdentifierValue_with_IdentifierValue_Then_the_HttpClient_Headers_should_have_the_IdentifierValue()
        {
            // Arrange
            var expected = Guid.NewGuid().ToString("N");
            var fakeMessageHandler = new FakeMessageHandler();
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            _sut.SetIdentifierValue(expected);

            // Assert
            httpClient.DefaultRequestHeaders.TryGetValues(Defaults.IdentifierHeaderName, out var actual).Should().BeTrue();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Given_a_SetupResponseApiService_When_using_SetIdentifierValue_with_a_ClientStub_and_IdentifierValue_Then_the_HttpClient_Headers_should_have_the_IdentifierValue()
        {
            // Arrange
            var expected = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", "TestIdentifierHeaderName");
            var fakeMessageHandler = new FakeMessageHandler();
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            _sut.SetIdentifierValue(clientStub, expected);

            // Assert
            httpClient.DefaultRequestHeaders.TryGetValues(clientStub.IdentifierHeaderName, out var actual).Should().BeTrue();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_AddOrUpdateAsync_with_one_ResponseModel_Then_should_return_void()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.Created, "addorupdate");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(Defaults.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            await _sut.AddOrUpdateAsync(GetResponseModels().FirstOrDefault());

            // Assert
            fakeMessageHandler.CallCount("addorupdate").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_AddOrUpdateAsync_with_a_ClientStub_and_one_ResponseModel_Then_should_return_void()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.Created, "addorupdate");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(clientStub.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            await _sut.AddOrUpdateAsync(clientStub, GetResponseModels().FirstOrDefault());

            // Assert
            fakeMessageHandler.CallCount("addorupdate").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_AddOrUpdateAsync_with_a_ClientStub_and_multiple_ResponseModels_Then_should_return_void()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.Created, "addorupdate");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(clientStub.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            await _sut.AddOrUpdateAsync(clientStub, GetResponseModels());

            // Assert
            fakeMessageHandler.CallCount("addorupdate").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_AddOrUpdateAsync_with_null_ClientStub_and_multiple_ResponseModels_Then_an_ArgumentNullException_should_be_thrown()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.Created, "addorupdate");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(Defaults.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddOrUpdateAsync(null, GetResponseModels()));
            fakeMessageHandler.CallCount("addorupdate").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_AddOrUpdateAsync_without_Accept_Header_and_with_a_ClientStub_and_multiple_ResponseModels_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.Created, "addorupdate");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Add(clientStub.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddOrUpdateAsync(clientStub, GetResponseModels()));

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("addorupdate").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_AddOrUpdateAsync_without_IdentifierHeaderName_Header_and_with_a_ClientStub_and_multiple_ResponseModels_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.Created, "addorupdate");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var _sut = new SetupResponseApiService(httpClient);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddOrUpdateAsync(clientStub, GetResponseModels()));

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("addorupdate").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_using_AddOrUpdateAsync_with_a_ClientStub_and_multiple_ResponseModels_When_a_HttpStatusCode_BadRequest_returned_Then_an_HttpRequestException_should_be_thrown()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.BadRequest, "addorupdate");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(clientStub.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(() => _sut.AddOrUpdateAsync(clientStub, GetResponseModels()));

            // Assert
            fakeMessageHandler.CallCount("addorupdate").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_GetAsync_Then_should_return_the_registered_ResponseModels()
        {
            // Arrange
            var expected = GetResponseModels();
            var identifierValue = Guid.NewGuid().ToString("N");
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, expected, "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(Defaults.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            var actual = await _sut.GetAsync();

            // Assert
            actual.Should().BeEquivalentTo(expected, options => options.Excluding(model => model.Result));
            fakeMessageHandler.CallCount("getall").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_GetAsync_with_a_ClientStub_Then_should_return_the_registered_ResponseModels()
        {
            // Arrange
            var expected = GetResponseModels();
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, expected, "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(clientStub.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            var actual = await _sut.GetAsync(clientStub);

            // Assert
            actual.Should().BeEquivalentTo(expected, options => options.Excluding(model => model.Result));
            fakeMessageHandler.CallCount("getall").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_GetAsync_with_null_ClientStub_Then_an_ArgumentNullException_should_be_thrown()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, GetResponseModels(), "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(Defaults.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetAsync(null));
            fakeMessageHandler.CallCount("getall").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_GetAsync_without_Accept_Header_and_with_a_ClientStub_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, GetResponseModels(), "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Add(clientStub.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetAsync(clientStub));

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("getall").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_GetAsync_without_IdentifierHeaderName_Header_and_with_a_ClientStub_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, GetResponseModels(), "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var _sut = new SetupResponseApiService(httpClient);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetAsync(clientStub));

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("getall").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_using_GetAsync_with_a_ClientStub_When_a_HttpStatusCode_BadRequest_returned_Then_an_HttpRequestException_should_be_thrown()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.BadRequest, GetResponseModels(), "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(clientStub.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(() => _sut.GetAsync(clientStub));

            // Assert
            fakeMessageHandler.CallCount("getall").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_the_default_RemoveAsync_Then_void_should_be_returned()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.NoContent, "remove");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(Defaults.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            await _sut.RemoveAsync();

            // Assert
            fakeMessageHandler.CallCount("remove").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_RemoveAsync_with_a_new_ClientStub_Then_should_return_void()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", "TestIdentifierHeaderName", Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.NoContent, "remove");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(clientStub.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act
            await _sut.RemoveAsync(clientStub);

            // Assert
            fakeMessageHandler.CallCount("remove").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_RemoveAsync_with_null_Then_an_ArgumentNullException_should_be_thrown()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.NoContent, "remove");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(Defaults.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RemoveAsync(null));
            fakeMessageHandler.CallCount("remove").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_RemoveAsync_without_Accept_Header_and_with_a_ClientStub_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.NoContent, "remove");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Add(clientStub.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => _sut.RemoveAsync(clientStub));

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("remove").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_RemoveAsync_without_IdentifierHeaderName_Header_and_with_a_ClientStub_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.NoContent, "remove");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var _sut = new SetupResponseApiService(httpClient);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => _sut.RemoveAsync(clientStub));

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("remove").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_SetupResponseApiService_When_using_RemoveAsync_with_a_new_ClientStub_and_return_HttpError_Then_an_HttpRequestException_should_be_thrown()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", "TestIdentifierHeaderName", Defaults.ResponseTtlInMinutes, Defaults.RequestTtlInMinutes);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.BadRequest, "remove");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(clientStub.IdentifierHeaderName, identifierValue);
            var _sut = new SetupResponseApiService(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _sut.RemoveAsync(clientStub));
            fakeMessageHandler.CallCount("remove").Should().Be(1);
        }

        private static IEnumerable<ResponseModel> GetResponseModels()
        {
            var responseModels = new List<ResponseModel>
            {
                new ResponseModel
                {
                    Path = "die/bla",
                    Query = new Dictionary<string, string>
                    {
                        { "Testing", "1" }
                    },
                    HttpMethods = new List<string>{ HttpMethod.Get.Method, HttpMethod.Post.Method },
                    ReturnStatusCode = HttpStatusCode.Created,
                    Result = new { CustomerId = 5, CustomerName = "Pepsi"}
                },
                new ResponseModel
                {
                    Path = "bla/{*die}/bla",
                    Query = new Dictionary<string, string>
                    {
                        { "Testing", "2" }
                    },
                    HttpMethods = new List<string>(),
                    ReturnStatusCode = HttpStatusCode.Created,
                    Headers = new Dictionary<string, IEnumerable<string>>
                    {
                        { "Accept", new []{ "application/json" } }
                    },
                    Location = new Uri("/testing/2", UriKind.Relative),
                    Result = new { CustomerId = 5, CustomerName = "Pepsi" }
                }
            };

            return responseModels;
        }
    }
}
