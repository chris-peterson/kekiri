using System.Collections.Generic;
using System.Linq;

namespace Kekiri
{
    internal static class LinqExtensions
    {
        public static IDictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> pairs, IEqualityComparer<K> keyComparer)
        {
            return pairs.ToDictionary(pair => pair.Key, pair => pair.Value, keyComparer);
        }
    }
}