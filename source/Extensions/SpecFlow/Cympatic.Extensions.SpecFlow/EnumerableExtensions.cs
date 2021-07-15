using System;
using System.Collections.Generic;

namespace Cympatic.Extensions.SpecFlow
{
    public static class EnumerableExtensions
    {
        public static int GetHashCodeOfElements<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (comparer == null)
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
