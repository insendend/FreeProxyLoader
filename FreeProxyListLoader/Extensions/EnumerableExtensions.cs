using System.Collections.Generic;

namespace FreeProxyListLoader.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> InsertFront<T>(this IEnumerable<T> col, T firstItem)
        {
            yield return firstItem;

            foreach (var item in col)
                yield return item;
        }
    }
}
