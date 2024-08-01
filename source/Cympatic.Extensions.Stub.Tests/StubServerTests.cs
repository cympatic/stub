using Cympatic.Extensions.Stub.Services;
using FluentAssertions;

namespace Cympatic.Extensions.Stub.Tests;

public class StubServerTests
{
    private readonly StubServer stubServer;

    public StubServerTests()
    {
        stubServer = new StubServer();
    }

    [Fact]
    public async Task TestMethod1()
    { 
        var apiService = stubServer.CreateApiService<SetupResponseApiService>();

        var list = await apiService.GetAllAsync();

        list.Should().BeEmpty();
    }
}
