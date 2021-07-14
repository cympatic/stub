using System;
using System.Linq;

namespace Cympatic.Extensions.SpecFlow
{
    public static class StringExtensions
    {
        public static Type GetSpecFlowItemType(this string name)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(type => type.GetSpecFlowItemName().Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
