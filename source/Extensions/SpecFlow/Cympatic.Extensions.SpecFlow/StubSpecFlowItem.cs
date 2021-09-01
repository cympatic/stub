using Cympatic.Extensions.SpecFlow.Attributes;
using Cympatic.Extensions.SpecFlow.Interfaces;

namespace Cympatic.Extensions.SpecFlow
{
    public abstract class StubSpecFlowItem : SpecFlowItem, IStubSpecFlowItem
    {
        [Ignore]
        public StubUrl ResponseToUrl { get; protected set; }

        [Ignore]
        public StubUrl ResponseToUrlScalar { get; protected set; }
    }
}
