namespace Cympatic.Extensions.Stub.SpecFlow.Interfaces;

public interface IStubSpecFlowItem : ISpecFlowItem
{
    StubUrl ResponseToUrl { get; }

    StubUrl ResponseToUrlScalar { get; }
}