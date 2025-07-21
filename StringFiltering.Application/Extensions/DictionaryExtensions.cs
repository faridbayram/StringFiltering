using System.Collections.Concurrent;

namespace StringFiltering.Application.Extensions;

public static class DictionaryExtensions {
    public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(
        this IEnumerable<TValue> source,
        Func<TValue, TKey> keySelector) where TKey : notnull =>
        new(
            source.Select(item => new KeyValuePair<TKey, TValue>(keySelector(item), item))
        );
}
