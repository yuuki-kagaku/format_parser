using System.Collections.Concurrent;

namespace FormatParser.Helpers;

public static class ConcurrentDictionaryExtensions
{
    public static TValue GetOrNew<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key) where TValue : new() where TKey : notnull =>
        dictionary.GetOrAdd(key, _ => new TValue());
}