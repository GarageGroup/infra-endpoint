using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GGroupp.Infra.Endpoint;

public sealed class EndpointEnumJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        Debug.Assert(typeToConvert is not null);
        return InnerCanConvert(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert is not null);
        Debug.Assert(InnerCanConvert(typeToConvert));

        var converter = (JsonConverter?)Activator.CreateInstance(
            type: typeof(EndpointEnumJsonConverter<>).MakeGenericType(typeToConvert));

        Debug.Assert(converter is not null);
        return converter;
    }

    private static bool InnerCanConvert(Type typeToConvert)
        =>
        typeToConvert.IsEnum;
}