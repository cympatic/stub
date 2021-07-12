using Cympatic.Extensions.SpecFlow.Interfaces;
using System.Collections.Generic;

namespace Cympatic.Extensions.SpecFlow
{
    public class StubItem
    {
        private readonly List<ISpecFlowItem> _items = new();

        public StubUrl ResponseToUrl { get; set; }

        public IEnumerable<ISpecFlowItem> Items { get => _items; }

        public void AddItem(ISpecFlowItem item)
        {
            _items.Add(item);
        }

        public void AddItems(IEnumerable<ISpecFlowItem> items)
        {
            _items.AddRange(items);
        }
    }
}
