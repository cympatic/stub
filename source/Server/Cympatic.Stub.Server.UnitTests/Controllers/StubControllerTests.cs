using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Controllers;
using Cympatic.Stub.Server.Interfaces;
using Cympatic.Stub.Server.UnitTests.TestData;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace Cympatic.Stub.Server.UnitTests.Controllers
{
    public class StubControllerTests
    {
        private readonly StubController _sut;
        private readonly IClientContainer _clientContainer;
        private readonly DefaultHttpContext _httpContext;
        private readonly string _path = Guid.NewGuid().ToString("N");

        public StubControllerTests()
        {
            _httpContext = new DefaultHttpContext();
            _httpContext.Request.RouteValues.Add("client", Guid.NewGuid().ToString("N"));
            _httpContext.Request.RouteValues.Add("slug", _path);
            _httpContext.Request.Method = HttpMethod.Get.Method;
            _httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "Testing", "1" },
                { "Wildcarded", Guid.NewGuid().ToString("N") }
            });

            var controllerContext = new ControllerContext()
            {
                HttpContext = _httpContext,
            };

            _clientContainer = Mock.Of<IClientContainer>();
            _sut = new StubController(_clientContainer, Mock.Of<ILogger<StubController>>())
            {
                ControllerContext = controllerContext
            };
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Call_Successfully(int indexResponseModel)
        {
            var responseModel = ResponseModelTestData.GetResponseModels().ToArray()[indexResponseModel];
            var requestModel = RequestModelTestData.GetRequestModel();
            var expectedResult = new ObjectResult(responseModel.Result)
            {
                StatusCode = (int)responseModel.ReturnStatusCode
            };

            Mock.Get(_clientContainer)
                .Setup(_ => _.FindResult(_httpContext.Request.Method, _path, _httpContext.Request.Query))
                .Returns(responseModel)
                .Verifiable();
            Mock.Get(_clientContainer)
                .Setup(_ => _.AddRequest(
                    _path,
                    It.IsAny<IDictionary<string, string>>(),
                    _httpContext.Request.Method,
                    It.IsAny<IDictionary<string, IEnumerable<string> >>(),
                    string.Empty,
                    true))
                .Returns(requestModel)
                .Verifiable();

            var actual = _sut.Call();

            actual.Result.Should().BeEquivalentTo(expectedResult);

            if (responseModel.Location != null)
            {
                _httpContext.Response.Headers.TryGetValue("location", out var location);
                location[0].Should().Be(responseModel.Location?.ToString());
            }

            if (responseModel.Headers != null)
            {
                foreach (var (key, values) in responseModel.Headers)
                {
                    _httpContext.Response.Headers.Should().Contain(key, values.ToArray());
                }
            }

            Mock.Get(_clientContainer)
                .Verify(_ => _.FindResult(_httpContext.Request.Method, _path, _httpContext.Request.Query), Times.Once);
            Mock.Get(_clientContainer)
                .Verify(_ => _.AddRequest(
                    _path,
                    It.IsAny<IDictionary<string, string>>(),
                    _httpContext.Request.Method,
                    It.IsAny<IDictionary<string, IEnumerable<string>>>(),
                    string.Empty, 
                    true), Times.Once);
        }

        [Fact]
        public void Call_NotFound()
        {
            var requestModel = RequestModelTestData.GetRequestModel();
            var expectedResult = new NotFoundResult();

            Mock.Get(_clientContainer)
                .Setup(_ => _.FindResult(_httpContext.Request.Method, _path, _httpContext.Request.Query))
                .Returns<ResponseModel>(default)
                .Verifiable();
            Mock.Get(_clientContainer)
                .Setup(_ => _.AddRequest(
                    _path,
                    It.IsAny<IDictionary<string, string>>(),
                    _httpContext.Request.Method,
                    It.IsAny<IDictionary<string, IEnumerable<string> >>(),
                    string.Empty,
                    false))
                .Returns(requestModel)
                .Verifiable();

            var actual = _sut.Call();

            actual.Result.Should().BeEquivalentTo(expectedResult);
            Mock.Get(_clientContainer)
                .Verify(_ => _.FindResult(_httpContext.Request.Method, _path, _httpContext.Request.Query), Times.Once);
            Mock.Get(_clientContainer)
                .Verify(_ => _.AddRequest(
                    _path,
                    It.IsAny<IDictionary<string, string>>(),
                    _httpContext.Request.Method,
                    It.IsAny<IDictionary<string, IEnumerable<string> >>(),
                    string.Empty,
                    false), Times.Once);
        }

        [Fact]
        public void Call_BadRequest_With_Exception()
        {
            var exception = new Exception("expected exception");
            var expected = new BadRequestObjectResult(exception.Message);

            Mock.Get(_clientContainer)
                .Setup(_ => _.FindResult(_httpContext.Request.Method, _path, _httpContext.Request.Query))
                .Throws(exception)
                .Verifiable();

            Mock.Get(_clientContainer)
                .Setup(_ => _.AddRequest(
                    _path,
                    It.IsAny<IDictionary<string, string>>(),
                    _httpContext.Request.Method,
                    It.IsAny<IDictionary<string, IEnumerable<string> >>(),
                    string.Empty,
                    false))
                .Verifiable();

            var actual = _sut.Call();

            actual.Result.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.FindResult(_httpContext.Request.Method, _path, _httpContext.Request.Query), Times.Once);
            Mock.Get(_clientContainer)
                .Verify(_ => _.AddRequest(
                    _path,
                    It.IsAny<IDictionary<string, string>>(),
                    _httpContext.Request.Method,
                    It.IsAny<IDictionary<string, IEnumerable<string> >>(),
                    string.Empty,
                    false), Times.Never);
        }
    }
}
