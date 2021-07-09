using Cympatic.Extensions.SpecFlow.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Cympatic.Extensions.SpecFlow
{
    public static class TableRowExtensions
    {
        public static ISpecFlowItem CreateInstance(this TableRow row, [NotNull] Type type, Dictionary<Type, IEnumerable<ISpecFlowItem>> relatedSpecFlowItems = default)
        {
            var instance = Activator.CreateInstance(type) as ISpecFlowItem;
            var table = row.ToTable();
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

        private static Table ToTable(this TableRow row)
        {
            var instanceTable = new Table(TableHeaderNames.Field, TableHeaderNames.Value);
            foreach (var kvp in row)
            {
                instanceTable.AddRow(kvp.Key, kvp.Value);
            }

            return instanceTable;
        }
    }
}
