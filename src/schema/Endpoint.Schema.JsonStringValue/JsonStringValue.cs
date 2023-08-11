using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

[JsonConverter(typeof(JsonStringValueConverterFactory))]
public readonly record struct JsonStringValue<TValue> : IOpenApiSchemaProvider, IEndpointTypeParser<JsonStringValue<TValue>>
    where TValue : struct
{
    public static OpenApiSchema GetSchema(bool nullable, IOpenApiAny? example = null, string? description = null)
        =>
        new()
        {
            Type = "string",
            Nullable = true,
            Example = example ?? new OpenApiString("{}"),
            Description = description
        };

    public static Result<JsonStringValue<TValue>, Failure<Unit>> Parse(string? source)
        =>
        string.IsNullOrEmpty(source) ? default(JsonStringValue<TValue>) : DeserializeOrFailure(source);

    private static Result<JsonStringValue<TValue>, Failure<Unit>> DeserializeOrFailure(string source)
    {
        try
        {
            return JsonSerializer.Deserialize<JsonStringValue<TValue>>(source, SerializerOptions);
        }
        catch (Exception ex)
        {
            return ex.ToFailure($"Un unexpected deserialization exception was thrown when deserializing JsonStringValue<{typeof(TValue).Name}>");
        }
    }

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public JsonStringValue(TValue value)
        =>
        Value = value;

    public TValue Value { get; }
}