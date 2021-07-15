using Cympatic.Extensions.SpecFlow.Interfaces;

namespace Cympatic.Extensions.SpecFlow
{
    public abstract class StubSpecFlowItem : SpecFlowItem, IStubSpecFlowItem
    {
        public StubUrl ResponseToUrl { get; protected set; }

        public StubUrl ResponseToUrlScalar { get; protected set; }
    }
}
