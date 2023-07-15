using Cympatic.Stub.Connectivity.Interfaces;
using Cympatic.Stub.Connectivity.Internal;
using Cympatic.Stub.Connectivity.Models;
using Cympatic.Stub.Server.Containers;
using Cympatic.Stub.Server.UnitTests.TestData;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace Cympatic.Stub.Server.UnitTests.Containers;

public class ClientContainerTest
{
    private readonly ClientContainer _sut;
    private readonly ResponseModelContainer _mockResponseModelContainer;
    private readonly RequestModelContainer _mockRequestModelContainer;
    private readonly IClientStub _clientStub;
    private readonly string _identifierValue;
    private readonly IHttpContextAccessor _mockHttpContextAccessor;
    private readonly HttpContext _context;

    public ClientContainerTest()
    {
        _clientStub = new ClientStub(Guid.NewGuid().ToString("N"), "UserId");
        _identifierValue = Guid.NewGuid().ToString("N");

        _mockHttpContextAccessor = Mock.Of<IHttpContextAccessor>();
        _context = new DefaultHttpContext();

        _mockResponseModelContainer = new Mock<ResponseModelContainer>(Mock.Of<ILogger<ResponseModelContainer>>()).Object;
        ResponseModelContainer responseModelContainerFactory() => _mockResponseModelContainer;

        _mockRequestModelContainer = new Mock<RequestModelContainer>(Mock.Of<ILogger<RequestModelContainer>>()).Object;
        RequestModelContainer requestModelContainerFactory() => _mockRequestModelContainer;

        _sut = new ClientContainer(
            _mockHttpContextAccessor,
            responseModelContainerFactory,
            requestModelContainerFactory,
            Mock.Of<ILogger<ClientContainer>>());
    }

    [Fact]
    public void Add_Client_SuccessFully()
    {
        var excepted = new ClientModel
        {
            Name = _clientStub.Name,
            IdentifierHeaderName = _clientStub.IdentifierHeaderName
        };

        _context.Request.RouteValues.Add("client", _clientStub.Name);
        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var actual = _sut.Add(_clientStub.IdentifierHeaderName, 1, 1);

        actual.Should().BeEquivalentTo(excepted);
    }

    [Fact]
    public void Add_Client_UnsuccessFully()
    {
        var excepted = default(ClientModel);

        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var actual = _sut.Add(_clientStub.IdentifierHeaderName, 1, 1);

        actual.Should().BeEquivalentTo(excepted);
    }

    [Fact]
    public void Add_Client_Without_IdentifierHeaderName_UnsuccessFully()
    {
        var excepted = default(ClientModel);

        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var actual = _sut.Add(string.Empty, 1, 1);

        actual.Should().BeEquivalentTo(excepted);
    }

    [Fact]
    public void Add_Client_Twice_SuccessFully()
    {
        var excepted = new ClientModel
        {
            Name = _clientStub.Name,
            IdentifierHeaderName = _clientStub.IdentifierHeaderName
        };

        _context.Request.RouteValues.Add("client", _clientStub.Name);
        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        _sut.Add(_clientStub.IdentifierHeaderName, 1, 1);
        var actual = _sut.Add(_clientStub.IdentifierHeaderName, 1, 1);

        actual.Should().BeEquivalentTo(excepted);
    }

    [Fact]
    public void Add_Client_Twice_UnsuccessFully()
    {
        var expectedErrorMessage = $"ClientName: {_clientStub.Name} already exists, but has differences (Parameter 'newClient')";

        _context.Request.RouteValues.Add("client", _clientStub.Name);
        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        _sut.Add(_clientStub.IdentifierHeaderName, 1, 1);
        var actual = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Add("IdentifierHeaderName", 1, 1));

        actual.Message.Should().Be(expectedErrorMessage);
    }

    [Fact]
    public void GetClient_SuccessFully()
    {
        _context.Request.RouteValues.Add("client", _clientStub.Name);
        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var excepted = _sut.Add(_clientStub.IdentifierHeaderName, 1, 1);

        var actual = _sut.GetClient();

        actual.Should().BeEquivalentTo(excepted);
    }

    [Fact]
    public void GetClient_UnsuccessFully()
    {
        var excepted = default(ClientModel);

        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var actual = _sut.GetClient();

        actual.Should().BeEquivalentTo(excepted);
    }

    [Fact]
    public void GetClient_Random_UnsuccessFully()
    {
        const string expectedErrorMessage = "Unable to determine the ClientInfo";

        _context.Request.RouteValues.Add("client", _clientStub.Name);
        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var excepted = _sut.Add(_clientStub.IdentifierHeaderName, 1, 1);

        _context.Request.RouteValues.Remove("client", out var _);
        _context.Request.RouteValues.Add("client", Guid.NewGuid().ToString("N"));
        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var actual = Assert.Throws<ArgumentException>(() => _sut.GetClient());

        actual.Message.Should().Be(expectedErrorMessage);
    }

    [Fact]
    public void GetClients_SuccessFully()
    {
        _context.Request.RouteValues.Add("client", _clientStub.Name);
        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var excepted = new List<ClientModel>
        {
            _sut.Add(_clientStub.IdentifierHeaderName, 1, 1)
        };

        var actual = _sut.GetClients();

        actual.Should().BeEquivalentTo(excepted);
    }

    [Fact]
    public void Remove_Client_SuccessFully()
    {
        _context.Request.RouteValues.Add("client", _clientStub.Name);
        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var actual = _sut.Add(_clientStub.IdentifierHeaderName, 1, 1);

        _sut.Remove();
    }

    [Fact]
    public void Remove_Client_UnsuccessFully()
    {
        const string expectedErrorMessage = "Unable to determine the clientName";

        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var actual = Assert.Throws<ArgumentException>(() => _sut.Remove());

        actual.Message.Should().Be(expectedErrorMessage);
    }

    [Fact]
    public void GetResponses_Successfully()
    {
        var expected = ResponseModelTestData.GetResponseModels();
        PrepareClientInContainer(_identifierValue);
        PrepareGetResponsesInContainer();

        var actual = _sut.GetResponses();

        actual.Should().BeEquivalentTo(expected);
        Mock.Get(_mockResponseModelContainer)
            .Verify(_ => _.Get(_identifierValue), Times.Once);
    }

    [Fact]
    public void GetResponses_Unsuccessfully()
    {
        var expected = $"IdentifierValue for header: \"{_clientStub.IdentifierHeaderName}\" not found!";
        PrepareClientInContainer(string.Empty);
        PrepareGetResponsesInContainer();

        var actual = Assert.Throws<ArgumentException>(() => _sut.GetResponses());

        actual.Message.Should().Be(expected);
        Mock.Get(_mockResponseModelContainer)
            .Verify(_ => _.Get(_identifierValue), Times.Never);
    }

    private void PrepareGetResponsesInContainer()
    {
        Mock.Get(_mockResponseModelContainer)
            .Setup(_ => _.Get(_identifierValue))
            .Returns(ResponseModelTestData.GetResponseModels())
            .Verifiable();
    }

    [Fact]
    public void AddOrUpdateResponses_Successfully()
    {
        PrepareClientInContainer(_identifierValue);
        var models = PrepareAddOrUpdateResponsesInContainer();

        _sut.AddOrUpdateResponses(models);

        Mock.Get(_mockResponseModelContainer)
            .Verify(_ => _.AddOrUpdate(_identifierValue, models), Times.Once);
    }

    [Fact]
    public void AddOrUpdateResponses_Unsuccessfully()
    {
        var expected = $"IdentifierValue for header: \"{_clientStub.IdentifierHeaderName}\" not found!";
        PrepareClientInContainer(string.Empty);
        var models = PrepareAddOrUpdateResponsesInContainer();

        var actual = Assert.Throws<ArgumentException>(() => _sut.AddOrUpdateResponses(models));

        actual.Message.Should().Be(expected);
        Mock.Get(_mockResponseModelContainer)
            .Verify(_ => _.AddOrUpdate(_identifierValue, models), Times.Never);
    }

    private IEnumerable<ResponseModel> PrepareAddOrUpdateResponsesInContainer()
    {
        var models = ResponseModelTestData.GetResponseModels();

        Mock.Get(_mockResponseModelContainer)
            .Setup(_ => _.AddOrUpdate(_identifierValue, models))
            .Verifiable();

        return models;
    }

    [Fact]
    public void RemoveResponses_Successfully()
    {
        PrepareClientInContainer(_identifierValue);
        PrepareRemoveResponsesInContainer();

        _sut.RemoveResponses();

        Mock.Get(_mockResponseModelContainer)
            .Verify(_ => _.Remove(_identifierValue), Times.Once);
    }

    [Fact]
    public void RemoveResponses_Unsuccessfully()
    {
        var expected = $"IdentifierValue for header: \"{_clientStub.IdentifierHeaderName}\" not found!";
        PrepareClientInContainer(string.Empty);
        PrepareRemoveResponsesInContainer();

        var actual = Assert.Throws<ArgumentException>(() => _sut.RemoveResponses());

        actual.Message.Should().Be(expected);
        Mock.Get(_mockResponseModelContainer)
            .Verify(_ => _.Remove(_identifierValue), Times.Never);
    }

    private void PrepareRemoveResponsesInContainer()
    {
        Mock.Get(_mockResponseModelContainer)
            .Setup(_ => _.Remove(_identifierValue))
            .Verifiable();
    }

    [Fact]
    public void GetRequests_Successfully()
    {
        PrepareClientInContainer(_identifierValue);
        var expected = PrepareGetRequestsInContainer();

        var actual = _sut.GetRequests();

        actual.Should().BeEquivalentTo(expected);
        Mock.Get(_mockRequestModelContainer)
            .Verify(_ => _.Get(_identifierValue), Times.Once);
    }

    [Fact]
    public void GetRequests_Unsuccessfully()
    {
        var expected = $"IdentifierValue for header: \"{_clientStub.IdentifierHeaderName}\" not found!";
        PrepareClientInContainer(string.Empty);
        PrepareGetRequestsInContainer();

        var actual = Assert.Throws<ArgumentException>(() => _sut.GetRequests());

        actual.Message.Should().Be(expected);
        Mock.Get(_mockRequestModelContainer)
            .Verify(_ => _.Get(_identifierValue), Times.Never);
    }

    private IEnumerable<RequestModel> PrepareGetRequestsInContainer()
    {
        var models = RequestModelTestData.GetRequestModels();

        Mock.Get(_mockRequestModelContainer)
            .Setup(_ => _.Get(_identifierValue))
            .Returns(models)
            .Verifiable();

        return models;
    }

    [Fact]
    public void RemoveRequests_Successfully()
    {
        PrepareClientInContainer(_identifierValue);
        PrepareRemoveRequestsInContainer();

        _sut.RemoveRequests();

        Mock.Get(_mockRequestModelContainer)
            .Verify(_ => _.Remove(_identifierValue), Times.Once);
    }

    [Fact]
    public void RemoveRequests_Unsuccessfully()
    {
        var expected = $"IdentifierValue for header: \"{_clientStub.IdentifierHeaderName}\" not found!";
        PrepareClientInContainer(string.Empty);
        PrepareRemoveRequestsInContainer();

        var actual = Assert.Throws<ArgumentException>(() => _sut.RemoveRequests());

        actual.Message.Should().Be(expected);
        Mock.Get(_mockRequestModelContainer)
            .Verify(_ => _.Remove(_identifierValue), Times.Never);
    }

    private void PrepareRemoveRequestsInContainer()
    {
        Mock.Get(_mockRequestModelContainer)
            .Setup(_ => _.Remove(_identifierValue))
            .Verifiable();
    }

    [Fact]
    public void AddRequest_Successfully()
    {
        PrepareClientInContainer(_identifierValue);
        var expected = PrepareAddRequestInContainer();

        var actual = _sut.AddRequest(expected.Path, expected.Query, expected.HttpMethod, expected.Headers, expected.Body, expected.ResponseFound);

        actual.Should().BeEquivalentTo(expected);
        Mock.Get(_mockRequestModelContainer)
            .Verify(_ => _.AddRequest(_identifierValue, expected.Path, expected.Query, expected.HttpMethod, expected.Headers, expected.Body, expected.ResponseFound), Times.Once);
    }

    [Fact]
    public void AddRequest_Unsuccessfully()
    {
        var expected = $"IdentifierValue for header: \"{_clientStub.IdentifierHeaderName}\" not found!";
        PrepareClientInContainer(string.Empty);
        var model = PrepareAddRequestInContainer();

        var actual = Assert.Throws<ArgumentException>(() => _sut.AddRequest(model.Path, model.Query, model.HttpMethod, model.Headers, model.Body, model.ResponseFound));

        actual.Message.Should().Be(expected);
        Mock.Get(_mockRequestModelContainer)
            .Verify(_ => _.AddRequest(_identifierValue, model.Path, model.Query, model.HttpMethod, model.Headers, model.Body, model.ResponseFound), Times.Never);
    }

    private RequestModel PrepareAddRequestInContainer()
    {
        var model = RequestModelTestData.GetRequestModel();

        Mock.Get(_mockRequestModelContainer)
            .Setup(_ => _.AddRequest(_identifierValue, It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<string>(), It.IsAny<IDictionary<string, IEnumerable<string> >>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(model)
            .Verifiable();

        return model;
    }

    [Fact]
    public void SearchRequests_Successfully()
    {
        var model = new RequestSearchModel
        {
            Path = "bla/{*die}/bla",
            Query = new Dictionary<string, string>
                {
                    { "Testing", "2" }
                },
            HttpMethods = new List<string> { HttpMethod.Get.Method, HttpMethod.Post.Method }
        };

        PrepareClientInContainer(_identifierValue);
        var expected = PrepareSearchRequestsInContainer();

        var actual = _sut.SearchRequests(model);

        actual.Should().BeEquivalentTo(expected);
        Mock.Get(_mockRequestModelContainer)
            .Verify(_ => _.Find(_identifierValue, model.Path, model.Query, model.HttpMethods), Times.Once);
    }

    [Fact]
    public void SearchRequests_Unsuccessfully()
    {
        var expected = $"IdentifierValue for header: \"{_clientStub.IdentifierHeaderName}\" not found!";
        var model = new RequestSearchModel
        {
            Path = "bla/{*die}/bla",
            Query = new Dictionary<string, string>
                {
                    { "Testing", "2" }
                },
            HttpMethods = new List<string> { HttpMethod.Get.Method, HttpMethod.Post.Method }
        };
        PrepareClientInContainer(string.Empty);
        PrepareSearchRequestsInContainer();

        var actual = Assert.Throws<ArgumentException>(() => _sut.SearchRequests(model));

        actual.Message.Should().Be(expected);
        Mock.Get(_mockRequestModelContainer)
            .Verify(_ => _.Find(_identifierValue, model.Path, model.Query, model.HttpMethods), Times.Never);
    }

    private IEnumerable<RequestModel> PrepareSearchRequestsInContainer()
    {
        var models = RequestModelTestData.GetRequestModels();
        Mock.Get(_mockRequestModelContainer)
            .Setup(_ => _.Find(_identifierValue, It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<IList<string>>()))
            .Returns(models)
            .Verifiable();

        return models;
    }

    private void PrepareClientInContainer(string identifierValue)
    {
        var excepted = new ClientModel
        {
            Name = _clientStub.Name,
            IdentifierHeaderName = _clientStub.IdentifierHeaderName
        };

        _context.Request.RouteValues.Add("client", _clientStub.Name);
        _context.Request.Headers[_clientStub.IdentifierHeaderName] = identifierValue;
        Mock.Get(_mockHttpContextAccessor)
            .Setup(_ => _.HttpContext)
            .Returns(_context);

        var actual = _sut.Add(_clientStub.IdentifierHeaderName, 1, 1);

        actual.Should().BeEquivalentTo(excepted);
    }

    [Fact]
    public void FindResult_Succesfully()
    {
        PrepareClientInContainer(_identifierValue);
        var expected = PrepareFindResultInContainer();

        var httpMethod = HttpMethod.Post.Method;
        var path = "die/bla";
        var query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "Testing", "1" }
            });

        var actual = _sut.FindResult(httpMethod, path, query);

        actual.Should().BeEquivalentTo(expected);
        Mock.Get(_mockResponseModelContainer)
            .Verify(_ => _.FindResult(_identifierValue, httpMethod, path, query), Times.Once);
    }

    [Fact]
    public void FindResult_Unsuccessfully()
    {
        var expected = $"IdentifierValue for header: \"{_clientStub.IdentifierHeaderName}\" not found!";
        PrepareClientInContainer(string.Empty);
        PrepareFindResultInContainer();

        var httpMethod = HttpMethod.Post.Method;
        var path = "die/bla";
        var query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "Testing", "1" }
            });

        var actual = Assert.Throws<ArgumentException>(() => _sut.FindResult(httpMethod, path, query));

        actual.Message.Should().Be(expected);
        Mock.Get(_mockResponseModelContainer)
            .Verify(_ => _.FindResult(_identifierValue, httpMethod, path, query), Times.Never);
    }

    private ResponseModel PrepareFindResultInContainer()
    {
        var model = ResponseModelTestData.GetResponseModels().ToArray()[0];
        Mock.Get(_mockResponseModelContainer)
            .Setup(_ => _.FindResult(_identifierValue, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<QueryCollection>()))
            .Returns(model)
            .Verifiable();

        return model;
    }
}
