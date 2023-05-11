using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GarageGroup.Infra;

internal sealed class JsonStringValueConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        =>
        typeToConvert.IsGenericType &&
        typeToConvert.GetGenericTypeDefinition() == typeof(JsonStringValue<>) &&
        typeToConvert.GetGenericArguments()[0].IsValueType;

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        =>
        (JsonConverter?)Activator.CreateInstance(
            type: typeof(JsonStringValueConverter<>).MakeGenericType(new[] { typeToConvert.GetGenericArguments()[0] }),
            bindingAttr: BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null);
}