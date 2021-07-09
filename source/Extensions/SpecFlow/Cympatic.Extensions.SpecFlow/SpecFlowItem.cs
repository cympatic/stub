using Cympatic.Extensions.SpecFlow.Attributes;
using Cympatic.Extensions.SpecFlow.Interfaces;
using System.Text.Json.Serialization;

namespace Cympatic.Extensions.SpecFlow
{
    public class SpecFlowItem : ISpecFlowItem
    {
        [Ignore]
        [JsonIgnore]
        public string Alias { get; set; }

        public void ConnectSpecFlowItem(ISpecFlowItem item)
        { }
    }
}
