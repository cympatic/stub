using Cympatic.Extensions.Stub.IntegrationTests.Fixtures;
using Cympatic.Extensions.Stub.IntegrationTests.Services;
using Cympatic.Extensions.Stub.Models;
using Cympatic.Extensions.Stub.Services;
using FluentAssertions;
using System.Net;

namespace Cympatic.Extensions.Stub.IntegrationTests;

public class ReceivedRequestTests : IClassFixture<StubServerFixture>
{
    private const int NumberOfItems = 10;

    private readonly StubServerFixture _fixture;
    private readonly ReceivedRequestApiService _sut;
    private readonly StubServer _stubServer;
    private readonly TestApiService _testApiService;

    public ReceivedRequestTests(StubServerFixture fixture)
    {
        _fixture = fixture;
        _fixture.Clear();

        _sut = _fixture.ReceivedRequestApiService;
        _stubServer = _fixture.StubServer;
        _testApiService = _fixture.TestApiService;
    }

    [Fact]
    public async Task When_Multiple_ReceivedRequest_are_recorded_Then_All_return_all_recorded_ReceivedRequest()
    {
        // Arrange
        var expected = new List<ReceivedRequest>();
        var tasks = new List<Task>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            var item = _fixture.GenerateReceivedRequest();
            expected.Add(item);

            tasks.Add(_testApiService.SendAsync(new Uri(item.Path, UriKind.Relative).WithParameters(item.Query), ConvertHttpMethod(item.HttpMethod), item.Headers));
        }
        await Task.WhenAll(tasks);

        // Act
        var actual = await _sut.GetAllAsync();

        // Assert
        actual.ToList().ForEach(_ => PrepareHeadersForValidation(_.Headers));
        actual.Should().BeEquivalentTo(expected, options => options
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task When_Matching_ResponseSetup_for_ReceivedRequest_is_recorded_Then_Find_return_the_recorded_ReceivedRequest()
    {
        // Arrange
        var list = new List<ReceivedRequest>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            list.Add(_fixture.GenerateReceivedRequest());
        }

        var expected = list[Random.Shared.Next(list.Count)];
        await _stubServer.AddResponseSetupAsync(new ResponseSetup
        {
            Path = expected.Path,
            HttpMethods = [expected.HttpMethod],
            Query = expected.Query,
            Headers = expected.Headers,
            ReturnStatusCode = HttpStatusCode.OK
        });

        var tasks = new List<Task>();
        foreach (var item in list)
        {
            tasks.Add(_testApiService.SendAsync(new Uri(item.Path, UriKind.Relative).WithParameters(item.Query), ConvertHttpMethod(item.HttpMethod), item.Headers));
        }
        await Task.WhenAll(tasks);

        // Act
        var actual = await _sut.FindAsync(new ReceivedRequestSearchParams(expected.Path, expected.Query, [expected.HttpMethod]));

        // Assert
        actual.ToList().ForEach(_ => PrepareHeadersForValidation(_.Headers));
        actual.Should().BeEquivalentTo([expected], options => options
            .Excluding(_ => _.FoundMatchingResponse)
            .Excluding(_ => _.Id)
            .Excluding(_ => _.CreatedDateTime));
    }

    [Fact]
    public async Task When_no_Matching_ResponseSetup_for_ReceivedRequest_is_recorded_Then_Find_return_an_empty_Enumerable()
    {
        // Arrange
        var tasks = new List<Task>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            var item = _fixture.GenerateReceivedRequest();
            tasks.Add(_testApiService.SendAsync(new Uri(item.Path, UriKind.Relative).WithParameters(item.Query), ConvertHttpMethod(item.HttpMethod), item.Headers));
        }
        await Task.WhenAll(tasks);

        // Act
        var dummy = _fixture.GenerateReceivedRequest();
        var actual = await _sut.FindAsync(new ReceivedRequestSearchParams(dummy.Path, dummy.Query, [dummy.HttpMethod]));

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task When_recorced_ReceivedRequest_are_removed_Then_All_return_an_empty_Enumerable()
    {
        // Arrange
        var tasks = new List<Task>();
        for (var i = 0; i < NumberOfItems; i++)
        {
            var item = _fixture.GenerateReceivedRequest();
            tasks.Add(_testApiService.SendAsync(new Uri(item.Path, UriKind.Relative).WithParameters(item.Query), ConvertHttpMethod(item.HttpMethod), item.Headers));
        }
        await Task.WhenAll(tasks);

        // Act
        await _sut.RemoveAllAsync();

        // Assert
        var actual = await _sut.GetAllAsync();
        actual.Should().BeEmpty();
    }

    private static HttpMethod ConvertHttpMethod(string httpMethod)
        => httpMethod.ToLowerInvariant() switch
        {
            "delete" => HttpMethod.Delete,
            "get" => HttpMethod.Get,
            "head" => HttpMethod.Head,
            "options" => HttpMethod.Options,
            "patch" => HttpMethod.Patch,
            "post" => HttpMethod.Post,
            "put" => HttpMethod.Put,
            "trace" => HttpMethod.Trace,
            _ => throw new InvalidCastException($"HttpMethod: '{httpMethod}' is an invalid value!")
        };

    private static void PrepareHeadersForValidation(IDictionary<string, IEnumerable<string?>> headers)
    {
        headers.Remove("Accept");
        headers.Remove("Host");
        headers.Remove("Content-Length");
    }
}
