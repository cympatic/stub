using System;
using System.Linq;

namespace Cympatic.Extensions.Stub.SpecFlow;

public static class StringExtensions
{
    public static Type GetSpecFlowItemType(this string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(type => type.GetSpecFlowItemName().Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
