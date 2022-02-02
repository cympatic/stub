using Cympatic.Extensions.Stub.SpecFlow;
using Cympatic.Extensions.Stub.SpecFlow.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Cympatic.Extensions.Stub.SpecFlow.Contexts
{
    public class ItemContext
    {
        public Dictionary<string, IEnumerable<ISpecFlowItem>> Items { get; } = new();

        public IEnumerable<ISpecFlowItem> Transform([NotNull] string itemName, [NotNull] Table table)
        {
            var type = ItemNameToType(itemName);
            return CreateSet(type, table);
        }

        public void Register([NotNull] string itemName, [NotNull] Table table)
        {
            var type = ItemNameToType(itemName);
            var specFlowItems = CreateSet(type, table).ToList();
            if (Items.TryGetValue(type.FullName, out var knownSpecFlowItems))
            {
                Items.Remove(type.FullName);
                specFlowItems.AddRange(knownSpecFlowItems);
            }

            Items.Add(type.FullName, specFlowItems);
        }

        public IList CreateList([NotNull] string itemName, [NotNull] Table table)
        {
            var type = ItemNameToType(itemName);
            var container = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

            if (Items.TryGetValue(type.FullName, out var foundItems))
            {
                foreach (var row in table.Rows)
                {
                    var value = row[TableHeaderNames.Alias];
                    var item = foundItems.Where(item => item.Alias == value).FirstOrDefault();
                    if (item != null)
                    {
                        container.Add(item);
                    }
                }
            }
            return container;
        }

        private Dictionary<Type, IEnumerable<ISpecFlowItem>> GetRelatedSpecFlowItems([NotNull] Type type, [NotNull] Table table)
        {
            var relatedTypes = GetAllRelatedTypes(type);

            if (relatedTypes == default)
            {
                return new();
            }

            var relatedSpecFlowItems = new Dictionary<Type, IEnumerable<ISpecFlowItem>>();
            foreach (var relatedType in relatedTypes)
            {
                var columnAlias = relatedType.GetSpecFlowItemName();
                if (table.ContainsColumn(columnAlias) && Items.TryGetValue(relatedType.FullName, out var foundItems))
                {
                    relatedSpecFlowItems.Add(relatedType, foundItems);
                }
            }

            return relatedSpecFlowItems;
        }

        private static Type ItemNameToType(string itemName)
        {
            var type = itemName.GetSpecFlowItemType();
            if (type == null)
            {
                throw new ArgumentOutOfRangeException(nameof(itemName), "Type not found");
            }

            return type;
        }

        private static IEnumerable<Type> GetAllRelatedTypes([NotNull] Type type)
        {
            if (typeof(ISpecFlowItem).IsAssignableFrom(type))
            {
                return typeof(ISpecFlowItem).GetAllClassesOf();
            }

            return default;
        }

        private static bool IsVerticalTable(Table table)
        {
            return table.Header.Count == 2 && table.Header.First() == TableHeaderNames.Field && table.Header.Last() == TableHeaderNames.Value;
        }

        private IEnumerable<ISpecFlowItem> CreateSet([NotNull] Type type, [NotNull] Table table)
        {
            if (IsVerticalTable(table))
            {
                throw new InvalidOperationException("Vertical tables are not allowed");
            }

            if (!typeof(ISpecFlowItem).IsAssignableFrom(type))
            {
                return Enumerable.Empty<ISpecFlowItem>();
            }

            var relatedSpecFlowItems = GetRelatedSpecFlowItems(type, table);

            return table.CreateSet((row) =>
            {
                var instance = Activator.CreateInstance(type) as ISpecFlowItem;
                table.FillInstance(instance);

                foreach (var relatedSpecFlowItem in relatedSpecFlowItems)
                {
                    var key = relatedSpecFlowItem.Key.GetSpecFlowItemName();
                    if (row.TryGetValue( key, out var alias) && !string.IsNullOrEmpty(alias))
                    {
                        foreach (var item in relatedSpecFlowItem.Value.Where(specFlowItem => specFlowItem.Alias == alias))
                        {
                            instance.ConnectSpecFlowItem(item);
                        }
                    }
                }

                return instance;
            });
        }
    }
}
