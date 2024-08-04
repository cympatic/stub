using Cympatic.Extensions.Stub.IntegrationTests.Servers;

namespace Cympatic.Extensions.Stub.IntegrationTests;

public class StubServerTests : IDisposable
{
    private readonly StubServer _sut;
    private readonly TestServer _testServer;

    public StubServerTests()
    {
        _sut = new StubServer();
        _testServer = new TestServer();

        _testServer.SetBaseAddressExternalApi(_sut.BaseAddressStub);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Test1()
    {

    }
}