using System;
using System.Linq;

namespace Cympatic.Extensions.SpecFlow
{
    public static class StringExtensions
    {
        public static Type GetSpecFlowItemByName(this string name)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(type => type.GetSpecFlowItemName().Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
