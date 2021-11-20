namespace Cympatic.Extensions.Stub.SpecFlow.Interfaces
{
    public interface ISpecFlowItem
    {
        string Alias { get; set; }

        void ConnectSpecFlowItem(ISpecFlowItem item);
    }
}
