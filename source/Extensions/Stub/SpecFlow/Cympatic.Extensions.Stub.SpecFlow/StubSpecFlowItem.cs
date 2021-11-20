using Cympatic.Extensions.Stub.SpecFlow.Attributes;
using Cympatic.Extensions.Stub.SpecFlow.Interfaces;

namespace Cympatic.Extensions.Stub.SpecFlow
{
    public abstract class StubSpecFlowItem : SpecFlowItem, IStubSpecFlowItem
    {
        [Ignore]
        public StubUrl ResponseToUrl { get; protected set; }

        [Ignore]
        public StubUrl ResponseToUrlScalar { get; protected set; }
    }
}
