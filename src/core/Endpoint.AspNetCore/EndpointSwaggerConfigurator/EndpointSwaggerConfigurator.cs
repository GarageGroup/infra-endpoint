using System.Collections.Generic;
using System.Linq;

namespace GarageGroup.Infra.Endpoint;

internal static partial class EndpointSwaggerConfigurator
{
    private static TDictionary Insert<TDictionary, TKey, TValue>(this TDictionary source, TKey key, TValue value)
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull
    {
        var result = new TDictionary
        {
            { key, value }
        };

        foreach (var kv in source)
        {
            result.Add(kv.Key, kv.Value);
        }

        return result;
    }

    private static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        where TKey : notnull
    {
        return source.ToDictionary(GetKey, GetValue);

        static TKey GetKey(KeyValuePair<TKey, TValue> kv) => kv.Key;

        static TValue GetValue(KeyValuePair<TKey, TValue> kv) => kv.Value;
    }
}