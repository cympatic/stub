using Cympatic.Stub.Connectivity.Constants;
using Cympatic.Stub.Connectivity.Internal;
using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Connectivity.UnitTests.Fakes;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cympatic.Stub.Connectivity.UnitTests
{
    public class VerifyRequestApiServiceTests
    {
        [Fact]
        public void Given_a_VerifyRequestApiService_When_using_SetIdentifierValue_with_IdentifierValue_Then_the_HttpClient_Headers_should_have_the_IdentifierValue()
        {
            // Arrange
            var expected = Guid.NewGuid().ToString("N");
            var fakeMessageHandler = new FakeMessageHandler();
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var sut = new VerifyRequestApiService(httpClient);

            // Act
            sut.SetIdentifierValue(expected);

            // Assert
            httpClient.DefaultRequestHeaders.TryGetValues(Defaults.IdentifierHeaderName, out var actual).Should().BeTrue();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Given_a_VerifyRequestApiService_When_using_SetIdentifierValue_with_a_ClientStub_and_IdentifierValue_Then_the_HttpClient_Headers_should_have_the_IdentifierValue()
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
            var sut = new VerifyRequestApiService(httpClient);

            // Act
            sut.SetClientStubIdentifierValue(clientStub, expected);

            // Assert
            httpClient.DefaultRequestHeaders.TryGetValues(clientStub.IdentifierHeaderName, out var actual).Should().BeTrue();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_GetAsync_Then_should_return_the_registered_ResponseModels()
        {
            // Arrange
            var expected = GetRequestModels();
            var identifierValue = Guid.NewGuid().ToString("N");
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, expected, "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(Defaults.IdentifierHeaderName, identifierValue);
            var sut = new VerifyRequestApiService(httpClient);

            // Act
            var actual = await sut.GetAsync();

            // Assert
            actual.Should().BeEquivalentTo(expected, options => options.Excluding(model => model.Body));
            fakeMessageHandler.CallCount("getall").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_GetAsync_with_a_ClientStub_Then_should_return_the_registered_ResponseModels()
        {
            // Arrange
            var expected = GetRequestModels();
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, expected, "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var sut = new VerifyRequestApiService(httpClient);
            sut.SetClientStubIdentifierValue(clientStub, identifierValue);

            // Act
            var actual = await sut.GetAsync();

            // Assert
            actual.Should().BeEquivalentTo(expected, options => options.Excluding(model => model.Body));
            fakeMessageHandler.CallCount("getall").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_GetAsync_without_Accept_Header_and_with_a_ClientStub_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, GetRequestModels(), "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            var sut = new VerifyRequestApiService(httpClient);
            sut.SetClientStubIdentifierValue(clientStub, identifierValue);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.GetAsync());

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("getall").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_GetAsync_without_IdentifierHeaderName_Header_and_with_a_ClientStub_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, GetRequestModels(), "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var sut = new VerifyRequestApiService(httpClient);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.GetAsync());

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("getall").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_using_GetAsync_with_a_ClientStub_When_a_HttpStatusCode_BadRequest_returned_Then_an_HttpRequestException_should_be_thrown()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.BadRequest, GetRequestModels(), "getall");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var sut = new VerifyRequestApiService(httpClient);
            sut.SetClientStubIdentifierValue(clientStub, identifierValue);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetAsync());

            // Assert
            fakeMessageHandler.CallCount("getall").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_the_default_RemoveAsync_Then_void_should_be_returned()
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
            var sut = new VerifyRequestApiService(httpClient);

            // Act
            await sut.RemoveAsync();

            // Assert
            fakeMessageHandler.CallCount("remove").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_RemoveAsync_with_a_new_ClientStub_Then_should_return_void()
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
            var sut = new VerifyRequestApiService(httpClient);
            sut.SetClientStubIdentifierValue(clientStub, identifierValue);

            // Act
            await sut.RemoveAsync();

            // Assert
            fakeMessageHandler.CallCount("remove").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_RemoveAsync_without_Accept_Header_and_with_a_ClientStub_Then_an_ArgumentException_should_be_thrown()
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
            var sut = new VerifyRequestApiService(httpClient);
            sut.SetClientStubIdentifierValue(clientStub, identifierValue);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.RemoveAsync());

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("remove").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_RemoveAsync_without_IdentifierHeaderName_Header_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.NoContent, "remove");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var sut = new VerifyRequestApiService(httpClient);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.RemoveAsync());

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("remove").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_RemoveAsync_with_a_new_ClientStub_and_return_HttpError_Then_an_HttpRequestException_should_be_thrown()
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
            var sut = new VerifyRequestApiService(httpClient);
            sut.SetClientStubIdentifierValue(clientStub, identifierValue);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => sut.RemoveAsync());
            fakeMessageHandler.CallCount("remove").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_SearchAsync_with_a_ClientStub_and_RequestSearchModel_Then_should_return_RequestModels()
        {
            // Arrange
            var expected = GetRequestModels();
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, expected, "search");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var sut = new VerifyRequestApiService(httpClient);
            sut.SetClientStubIdentifierValue(clientStub, identifierValue);

            // Act
            var actual = await sut.SearchAsync(GetRequestSearchModel());

            // Assert
            actual.Should().BeEquivalentTo(expected, options => options.Excluding(model => model.Body));
            fakeMessageHandler.CallCount("search").Should().Be(1);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_SearchAsync_without_Accept_Header_and_with_a_ClientStub_and_RequestSearchModel_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, GetRequestModels(), "search");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            var sut = new VerifyRequestApiService(httpClient);
            sut.SetClientStubIdentifierValue(clientStub, identifierValue);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.SearchAsync(GetRequestSearchModel()));

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("search").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_When_using_SearchAsync_without_IdentifierHeaderName_Header_and_RequestSearchModel_Then_an_ArgumentException_should_be_thrown()
        {
            // Arrange
            const string expected = "HttpClient doesn't contain the correct headers";
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.OK, GetRequestModels(), "search");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var sut = new VerifyRequestApiService(httpClient);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.SearchAsync(GetRequestSearchModel()));

            // Assert
            actual.Message.Should().BeEquivalentTo(expected);
            fakeMessageHandler.CallCount("search").Should().Be(0);
        }

        [Fact]
        public async Task Given_a_VerifyRequestApiService_using_SearchAsync_with_a_ClientStub_and_RequestSearchModel_When_a_HttpStatusCode_BadRequest_returned_Then_an_HttpRequestException_should_be_thrown()
        {
            // Arrange
            var identifierValue = Guid.NewGuid().ToString("N");
            var clientStub = new ClientStub("TestClientStub", Defaults.IdentifierHeaderName);
            var fakeMessageHandler = new FakeMessageHandler(HttpStatusCode.BadRequest, "search");
            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = new Uri("http://fake.cympatic.com")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var sut = new VerifyRequestApiService(httpClient);
            sut.SetClientStubIdentifierValue(clientStub, identifierValue);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(() => sut.SearchAsync(GetRequestSearchModel()));

            // Assert
            fakeMessageHandler.CallCount("search").Should().Be(1);
        }

        public static RequestSearchModel GetRequestSearchModel()
        {
            return new RequestSearchModel
            {
                Path = "bla/{*die}/bla",
                Query = new Dictionary<string, string>
                    {
                        { "Testing", "2" }
                    },
                HttpMethods = new List<string> { HttpMethod.Get.Method, HttpMethod.Post.Method }
            };
        }

        public static IEnumerable<RequestModel> GetRequestModels()
        {
            var requestModels = new List<RequestModel>
            {
                new RequestModel
                {
                    Path = "segment1/wildcard/segment3",
                    Query = new Dictionary<string, string>
                    {
                        { "queryparam1", "1" }
                    },
                    HttpMethod = HttpMethod.Get.Method,
                    Body = JsonSerializer.Serialize(new { CustomerId = 5, CustomerName = "Pepsi"}),
                    ResponseFound = true
                },
                new RequestModel
                {
                    Path = "segment1/wildcard/segment3",
                    Query = new Dictionary<string, string>
                    {
                        { "queryparam1", "1" }
                    },
                    HttpMethod = HttpMethod.Post.Method,
                    Headers = new Dictionary<string, IEnumerable<string>>
                    {
                        { "Accept", new []{ "application/json" } }
                    },
                    Body = JsonSerializer.Serialize(new { CustomerId = 5, CustomerName = "Pepsi"}),
                    ResponseFound = true
                }
            };

            return requestModels;
        }
    }
}
