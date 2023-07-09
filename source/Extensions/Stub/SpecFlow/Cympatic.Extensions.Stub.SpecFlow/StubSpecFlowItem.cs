using Cympatic.Extensions.Stub.SpecFlow.Attributes;
using Cympatic.Extensions.Stub.SpecFlow.Interfaces;

namespace Cympatic.Extensions.Stub.SpecFlow;

public abstract class StubSpecFlowItem : SpecFlowItem, IStubSpecFlowItem
{
    protected virtual StubUrl GetResponseToUrl()
    { 
        return null;
    }

    protected virtual StubUrl GetResponseToUrlScalar()
    {
        return null;
    }

    [Ignore]
    public StubUrl ResponseToUrl { get => GetResponseToUrl(); }

    [Ignore]
    public StubUrl ResponseToUrlScalar { get => GetResponseToUrlScalar(); }
}
