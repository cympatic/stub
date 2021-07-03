using Cympatic.Stub.Abstractions.Models;
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
using Xunit;

namespace Cympatic.Stub.Server.UnitTests.Controllers
{
    public class VerifiyRequestControllerTests
    {
        private readonly string _clientName;
        private readonly VerifyRequestController _sut;
        private readonly IClientContainer _clientContainer;

        public VerifiyRequestControllerTests()
        {
            _clientName = Guid.NewGuid().ToString("N");
            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues.Add("client", _clientName);
            httpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "Testing", "1" },
                { "Wildcarded", Guid.NewGuid().ToString("N") }
            });

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            _clientContainer = Mock.Of<IClientContainer>();
            _sut = new VerifyRequestController(_clientContainer, Mock.Of<ILogger<VerifyRequestController>>())
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public void GetAll_Ok()
        {
            var models = RequestModelTestData.GetRequestModels();
            var expected = new OkObjectResult(models);

            Mock.Get(_clientContainer)
                .Setup(_ => _.GetRequests())
                .Returns(models)
                .Verifiable();

            var actual = _sut.GetAll(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.GetRequests(), Times.Once);
        }

        [Fact]
        public void GetAll_BadRequest_With_Exception()
        {
            var exception = new ArgumentException("Unable to determine the identifierValue");
            var expected = new BadRequestObjectResult(exception.Message);

            Mock.Get(_clientContainer)
                .Setup(_ => _.GetRequests())
                .Throws(exception)
                .Verifiable();

            var actual = _sut.GetAll(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.GetRequests(), Times.Once);
        }

        [Fact]
        public void Remove_NoContent()
        {
            var expected = new NoContentResult();

            Mock.Get(_clientContainer)
                .Setup(_ => _.RemoveRequests())
                .Verifiable();

            var actual = _sut.Remove(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.RemoveRequests(), Times.Once);
        }

        [Fact]
        public void Remove_BadRequest_With_Exception()
        {
            var exception = new ArgumentException("Unable to determine the identifierValue");
            var expected = new BadRequestObjectResult(exception.Message);

            Mock.Get(_clientContainer)
                .Setup(_ => _.RemoveRequests())
                .Throws(exception)
                .Verifiable();

            var actual = _sut.Remove(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.RemoveRequests(), Times.Once);
        }

        [Fact]
        public void Search_Ok()
        {
            var models = RequestModelTestData.GetRequestModels();
            var search = new RequestSearchModel();
            var expected = new OkObjectResult(models);

            Mock.Get(_clientContainer)
                .Setup(_ => _.SearchRequests(search))
                .Returns(models)
                .Verifiable();

            var actual = _sut.Search(_clientName, search);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.SearchRequests(search), Times.Once);
        }

        [Fact]
        public void Search_BadRequest_With_Exception()
        {
            var search = new RequestSearchModel();
            var exception = new ArgumentException("Unable to determine the identifierValue");
            var expected = new BadRequestObjectResult(exception.Message);

            Mock.Get(_clientContainer)
                .Setup(_ => _.SearchRequests(search))
                .Throws(exception)
                .Verifiable();

            var actual = _sut.Search(_clientName, search);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.SearchRequests(search), Times.Once);
        }
    }
}
