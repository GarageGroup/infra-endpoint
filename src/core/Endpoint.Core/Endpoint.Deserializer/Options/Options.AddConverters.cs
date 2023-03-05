using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static JsonSerializerOptions AddConverters(this JsonSerializerOptions options, params JsonConverter[] converters)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (converters?.Length is not > 0)
        {
            return options;
        }

        foreach (var converter in converters)
        {
            options.Converters.Add(converter);
        }

        return options;
    }
}