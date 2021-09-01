using Cympatic.Extensions.SpecFlow.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Cympatic.Extensions.SpecFlow
{
    public static class TableExtensions
    {
        public static object CreateInstance(this Table table, [NotNull] Type type, Dictionary<Type, IEnumerable<ISpecFlowItem>> relatedSpecFlowItems = default)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            var instance = Activator.CreateInstance(type) as ISpecFlowItem;
            table.FillInstance(instance);

            if (relatedSpecFlowItems != default && instance is ISpecFlowItem specFlowItem)
            {
                foreach (var relatedSpecFlowItem in relatedSpecFlowItems)
                {
                    var key = relatedSpecFlowItem.Key.GetSpecFlowItemName();
                    if (table.TryGetValue(key, out var alias) && !string.IsNullOrEmpty(alias))
                    {
                        foreach (var item in relatedSpecFlowItem.Value.Where(specFlowItem => specFlowItem.Alias == alias))
                        {
                            instance.ConnectSpecFlowItem(item);
                        }
                    }
                }
            }
            return instance;
        }

        public static IEnumerable<object> CreateSet(this Table table, [NotNull] Type type, Dictionary<Type, IEnumerable<ISpecFlowItem>> relatedSpecFlowItems = default)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            if (table.ContainsColumn(TableHeaderNames.Field) &&
                table.ContainsColumn(TableHeaderNames.Value))
            {
                return new List<object> { table.CreateInstance(type, relatedSpecFlowItems) };
            }

            return table.Rows.Select(row => row.CreateInstance(type, relatedSpecFlowItems));
        }

        public static IEnumerable<object> TransformToSpecFlowItems(this Table table, [NotNull] Type type, ScenarioContext scenarioContext)
        {
            var relatedSpecFlowItems = GetRelatedSpecFlowItems(table, type, scenarioContext);

            return CreateSet(table, type, relatedSpecFlowItems);
        }

        public static IEnumerable<object> TransformToSpecFlowItemsAndRegister(this Table table, [NotNull] Type type, ScenarioContext scenarioContext)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            var specFlowItems = TransformToSpecFlowItems(table, type, scenarioContext).ToList();

            if (scenarioContext.ContainsKey(type.FullName))
            {
                var knownSpecFlowItems = scenarioContext.Get<IEnumerable<object>>(type.FullName);
                specFlowItems.AddRange(knownSpecFlowItems);
            }

            scenarioContext.Set<IEnumerable<object>>(specFlowItems, type.FullName);

            return specFlowItems;
        }

        public static Dictionary<Type, IEnumerable<ISpecFlowItem>> GetRelatedSpecFlowItems(this Table table, [NotNull] Type type, ScenarioContext scenarioContext)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            var relatedTypes = GetAllRelatedTypes(type);

            if (relatedTypes == default)
            {
                return new();
            }

            var relatedSpecFlowItems = new Dictionary<Type, IEnumerable<ISpecFlowItem>>();
            foreach (var relatedType in relatedTypes)
            {
                var columnAlias = relatedType.GetSpecFlowItemName();
                if (table.ContainsColumn(columnAlias) && scenarioContext.ContainsKey(relatedType.FullName))
                {
                    relatedSpecFlowItems.Add(relatedType, scenarioContext.Get<IEnumerable<object>>(relatedType.FullName).OfType<ISpecFlowItem>());
                }
            }

            return relatedSpecFlowItems;
        }

        public static IList CreateContainer(this Table table, [NotNull] Type type, ScenarioContext scenarioContext)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            var container = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

            if (scenarioContext.ContainsKey(type.FullName))
            {
                var items = scenarioContext.Get<IEnumerable<object>>(type.FullName).OfType<ISpecFlowItem>();
                foreach (var row in table.Rows)
                {
                    var value = row[TableHeaderNames.Alias];
                    var item = items.Where(item => item.Alias == value).FirstOrDefault();
                    if (item != null)
                    {
                        container.Add(item);
                    }
                }
            }
            return container;
        }

        public static bool TryGetValue(this Table table, [NotNull] string name, out string value)
        {
            value = default;
            var row = table.Rows.FirstOrDefault(r => r[TableHeaderNames.Field].ToLowerInvariant() == name.ToLowerInvariant());

            if (row == null)
            {
                return false;
            }

            value = row[TableHeaderNames.Value];
            return true;
        }

        private static IEnumerable<Type> GetAllRelatedTypes([NotNull] Type type)
        {
            if (typeof(ISpecFlowItem).IsAssignableFrom(type))
            {
                return typeof(ISpecFlowItem).GetAllClassesOf();
            }

            return default;
        }
    }
}
