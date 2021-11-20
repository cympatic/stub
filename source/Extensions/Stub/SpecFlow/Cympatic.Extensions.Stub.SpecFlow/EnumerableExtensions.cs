using System;
using System.Collections.Generic;

namespace Cympatic.Extensions.Stub.SpecFlow
{
    public static class EnumerableExtensions
    {
        public static int GetHashCodeOfElements<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = default)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (comparer == default)
            {
                comparer = EqualityComparer<T>.Default;
            }

            var hash = 0;
            foreach (T element in source)
            {
                hash ^= comparer.GetHashCode(element);
            }

            return hash;
        }
    }
}
