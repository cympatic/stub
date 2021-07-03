using Cympatic.Stub.Abstractions.Models;
using Cympatic.Stub.Server.Controllers;
using Cympatic.Stub.Server.Interfaces;
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
    public class SetupClientControllerTests
    {
        private const string _identifierHeaderName = "UserId";

        private readonly string _clientName;
        private readonly SetupClientController _sut;
        private readonly IClientContainer _clientContainer;

        public SetupClientControllerTests()
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
            _sut = new SetupClientController(_clientContainer, Mock.Of<ILogger<SetupClientController>>())
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public void Add_Created()
        {
            var model = new ClientModel
            {
                Name = _clientName,
                IdentifierHeaderName = _identifierHeaderName
            };

            var expected = new CreatedAtActionResult("GetClient", null, new { client = model.Name }, model);

            Mock.Get(_clientContainer)
                .Setup(_ => _.Add(_identifierHeaderName, It.IsAny<int>(), It.IsAny<int>()))
                .Returns(model)
                .Verifiable();

            var actual = _sut.Add(_clientName, _identifierHeaderName, 10, 10);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.Add(_identifierHeaderName, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Add_BadRequest()
        {
            var expected = new BadRequestResult();

            Mock.Get(_clientContainer)
                .Setup(_ => _.Add(string.Empty, It.IsAny<int>(), It.IsAny<int>()))
                .Returns(default(ClientModel))
                .Verifiable();

            var actual = _sut.Add(_clientName, string.Empty, 10, 10);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.Add(string.Empty, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Add_BadRequest_With_Exception()
        {
            var exception = new ArgumentOutOfRangeException("newClient", $"ClientName: {Guid.NewGuid().ToString("N")} already exists");
            var expected = new BadRequestObjectResult(exception.Message);

            Mock.Get(_clientContainer)
                .Setup(_ => _.Add(_identifierHeaderName, It.IsAny<int>(), It.IsAny<int>()))
                .Throws(exception)
                .Verifiable();

            var actual = _sut.Add(_clientName, _identifierHeaderName, 10, 10);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.Add(_identifierHeaderName, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Remove_NoContent()
        {
            var expected = new NoContentResult();

            Mock.Get(_clientContainer)
                .Setup(_ => _.Remove())
                .Verifiable();

            var actual = _sut.Remove(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.Remove(), Times.Once);
        }

        [Fact]
        public void Remove_BadRequest_With_Exception()
        {
            var exception = new ArgumentException("Unable to determine the clientName");
            var expected = new BadRequestObjectResult(exception.Message);

            Mock.Get(_clientContainer)
                .Setup(_ => _.Remove())
                .Throws(exception)
                .Verifiable();

            var actual = _sut.Remove(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.Remove(), Times.Once);
        }

        [Fact]
        public void GetClient_Ok()
        {
            var model = new ClientModel
            {
                Name = _clientName,
                IdentifierHeaderName = _identifierHeaderName
            };

            var expected = new OkObjectResult(model);

            Mock.Get(_clientContainer)
                .Setup(_ => _.GetClient())
                .Returns(model)
                .Verifiable();

            var actual = _sut.GetClient(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.GetClient(), Times.Once);
        }

        [Fact]
        public void GetClient_BadRequest_With_Exception_Unable_To_Determine_ClientName()
        {
            var exception = new ArgumentException("Unable to determine the clientName");
            var expected = new BadRequestObjectResult(exception.Message);

            Mock.Get(_clientContainer)
                .Setup(_ => _.GetClient())
                .Throws(exception)
                .Verifiable();

            var actual = _sut.GetClient(Guid.NewGuid().ToString("N"));

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.GetClient(), Times.Once);
        }

        [Fact]
        public void GetClient_BadRequest_With_Exception_No_ClientName()
        {
            var expected = new BadRequestObjectResult(new ArgumentException("No clientName was given in route"));

            Mock.Get(_clientContainer)
                .Setup(_ => _.GetClient())
                .Verifiable();

            var actual = _sut.GetClient(string.Empty);

            actual.Should().BeAssignableTo(expected.GetType());
            Mock.Get(_clientContainer)
                .Verify(_ => _.GetClient(), Times.Never);
        }
    }
}
