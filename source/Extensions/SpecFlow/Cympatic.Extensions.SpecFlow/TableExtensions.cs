using Cympatic.Extensions.SpecFlow.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Cympatic.Extensions.SpecFlow
{
    public static class TableExtensions
    {
        public static ISpecFlowItem CreateInstance(this Table table, [NotNull] Type type, Dictionary<Type, IEnumerable<ISpecFlowItem>> relatedSpecFlowItems = default)
        {
            var instance = Activator.CreateInstance(type) as ISpecFlowItem;
            table.FillInstance(instance);

            if (relatedSpecFlowItems != default)
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

        public static IEnumerable<ISpecFlowItem> CreateSet(this Table table, [NotNull] Type type, Dictionary<Type, IEnumerable<ISpecFlowItem>> relatedSpecFlowItems = default)
        {
            if (table.ContainsColumn(TableHeaderNames.Field) &&
                table.ContainsColumn(TableHeaderNames.Value))
            {
                return new List<ISpecFlowItem> { table.CreateInstance(type, relatedSpecFlowItems) };
            }

            return table.Rows.Select(row => row.CreateInstance(type, relatedSpecFlowItems));
        }

        public static IEnumerable<ISpecFlowItem> TransformToSpecFlowItems(this Table table, [NotNull] Type type, ScenarioContext scenarioContext)
        {
            var relatedSpecFlowItems = GetRelatedSpecFlowItems(table, type, scenarioContext);

            return CreateSet(table, type, relatedSpecFlowItems);
        }

        public static IEnumerable<ISpecFlowItem> TransformToSpecFlowItemsAndRegisterInContext(this Table table, [NotNull] Type type, ScenarioContext scenarioContext)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var specFlowItems = TransformToSpecFlowItems(table, type, scenarioContext).ToList();

            if (scenarioContext.ContainsKey(type.FullName))
            {
                var knownSpecFlowItems = scenarioContext.Get<IEnumerable<ISpecFlowItem>>(type.FullName);
                specFlowItems.AddRange(knownSpecFlowItems);
            }

            scenarioContext.Set<IEnumerable<ISpecFlowItem>>(specFlowItems, type.FullName);

            return specFlowItems;
        }

        public static Dictionary<Type, IEnumerable<ISpecFlowItem>> GetRelatedSpecFlowItems(this Table table, [NotNull] Type type, ScenarioContext scenarioContext)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var relatedTypes = GetAllRelatedTypes(type);

            var relatedSpecFlowItems = new Dictionary<Type, IEnumerable<ISpecFlowItem>>();
            foreach (var relatedType in relatedTypes)
            {
                var columnAlias = relatedType.GetSpecFlowItemName();
                if (table.ContainsColumn(columnAlias) && scenarioContext.ContainsKey(relatedType.FullName))
                {
                    relatedSpecFlowItems.Add(relatedType, scenarioContext.Get<IEnumerable<ISpecFlowItem>>(relatedType.FullName));
                }
            }

            return relatedSpecFlowItems;
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

            throw new ArgumentOutOfRangeException(nameof(type), $"{type.Name} isn't devired from {nameof(ISpecFlowItem)}");
        }
    }
}
