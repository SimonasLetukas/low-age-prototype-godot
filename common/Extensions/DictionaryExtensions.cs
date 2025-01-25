using System.Collections.Generic;

namespace LowAgeCommon.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool IsEquivalentTo<TKey, TValue>(this Dictionary<TKey, TValue>? dict1,
            Dictionary<TKey, TValue>? dict2)
        {
            if (dict1 == dict2) return true;
            if (dict1 == null || dict2 == null) return false;
            if (dict1.Count != dict2.Count) return false;

            var valueComparer = EqualityComparer<TValue>.Default;

            foreach (var kvp in dict1)
            {
                if (!dict2.TryGetValue(kvp.Key, out var value) || !valueComparer.Equals(kvp.Value, value))
                    return false;
            }

            return true;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key,
            TValue defaultValue = default)
        {
            return dict.TryGetValue(key, out var value)
                ? value
                : defaultValue;
        }
    }
}