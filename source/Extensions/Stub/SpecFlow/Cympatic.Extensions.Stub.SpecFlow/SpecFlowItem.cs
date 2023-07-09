using Cympatic.Extensions.Stub.SpecFlow.Attributes;
using System.Text.Json.Serialization;
using Cympatic.Extensions.Stub.SpecFlow.Interfaces;

namespace Cympatic.Extensions.Stub.SpecFlow;

public abstract class SpecFlowItem : ISpecFlowItem
{
    [Ignore]
    [JsonIgnore]
    public string Alias { get; set; }

    public virtual void ConnectSpecFlowItem(ISpecFlowItem item)
    { }
}
