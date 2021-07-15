namespace Cympatic.Extensions.SpecFlow.Interfaces
{
    public interface ISpecFlowItem
    {
        string Alias { get; set; }

        void ConnectSpecFlowItem(ISpecFlowItem item);
    }
}
