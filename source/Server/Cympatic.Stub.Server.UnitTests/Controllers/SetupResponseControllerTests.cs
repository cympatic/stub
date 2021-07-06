using Cympatic.Stub.Server.Controllers;
using Cympatic.Stub.Server.Interfaces;
using Cympatic.Stub.Server.UnitTests.TestData;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Cympatic.Stub.Server.UnitTests.Controllers
{
    public class SetupResponseControllerTests
    {
        private readonly string _clientName;
        private readonly SetupResponseController _sut;
        private readonly IClientContainer _clientContainer;

        public SetupResponseControllerTests()
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
            _sut = new SetupResponseController(_clientContainer, Mock.Of<ILogger<SetupResponseController>>())
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public void GetAll_Ok()
        {
            var models = ResponseModelTestData.GetResponseModels();
            var expected = new OkObjectResult(models);

            Mock.Get(_clientContainer)
                .Setup(_ => _.GetResponses())
                .Returns(models)
                .Verifiable();

            var actual = _sut.GetAll(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.GetResponses(), Times.Once);
        }

        [Fact]
        public void GetAll_BadRequest_With_Exception()
        {
            var exception = new ArgumentException("Unable to determine the identifierValue");
            var expected = new BadRequestObjectResult(exception.Message);

            Mock.Get(_clientContainer)
                .Setup(_ => _.GetResponses())
                .Throws(exception)
                .Verifiable();

            var actual = _sut.GetAll(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.GetResponses(), Times.Once);
        }

        [Fact]
        public void Remove_NoContent()
        {
            var expected = new NoContentResult();

            Mock.Get(_clientContainer)
                .Setup(_ => _.RemoveResponses())
                .Verifiable();

            var actual = _sut.Remove(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.RemoveResponses(), Times.Once);
        }

        [Fact]
        public void Remove_BadRequest_With_Exception()
        {
            var exception = new ArgumentException("Unable to determine the identifierValue");
            var expected = new BadRequestObjectResult(exception.Message);

            Mock.Get(_clientContainer)
                .Setup(_ => _.RemoveResponses())
                .Throws(exception)
                .Verifiable();

            var actual = _sut.Remove(_clientName);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.RemoveResponses(), Times.Once);
        }

        [Fact]
        public void AddOrUpdate_CreateAtAction()
        {
            var models = ResponseModelTestData.GetResponseModels();
            var expected = new CreatedAtActionResult("GetAll", null, new { client = _clientName }, models);

            Mock.Get(_clientContainer)
                .Setup(_ => _.AddOrUpdateResponses(models))
                .Verifiable();

            var actual = _sut.AddOrUpdate(_clientName, models);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.AddOrUpdateResponses(models), Times.Once);
        }

        [Fact]
        public void AddOrUpdate_ModelState_Invalid()
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("error", "set modelstate to invalid");
            _sut.ModelState.Merge(modelState);

            var models = ResponseModelTestData.GetResponseModels();
            var expected = new BadRequestObjectResult(modelState);

            Mock.Get(_clientContainer)
                .Setup(_ => _.AddOrUpdateResponses(models))
                .Verifiable();

            var actual = _sut.AddOrUpdate(_clientName, models);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.AddOrUpdateResponses(models), Times.Never);
        }

        [Fact]
        public void AddOrUpdate_BadRequest_With_Exception()
        {
            var models = ResponseModelTestData.GetResponseModels();
            var exception = new ArgumentException("Unable to determine the identifierValue");
            var expected = new BadRequestObjectResult(exception.Message);

            Mock.Get(_clientContainer)
                .Setup(_ => _.AddOrUpdateResponses(models))
                .Throws(exception)
                .Verifiable();

            var actual = _sut.AddOrUpdate(_clientName, models);

            actual.Should().BeEquivalentTo(expected);
            Mock.Get(_clientContainer)
                .Verify(_ => _.AddOrUpdateResponses(models), Times.Once);
        }
    }
}
