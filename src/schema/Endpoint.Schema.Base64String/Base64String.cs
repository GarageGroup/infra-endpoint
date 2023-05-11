using System;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

[JsonConverter(typeof(Base64StringJsonConverter))]
public readonly record struct Base64String : IOpenApiSchemaProvider
{
    public static OpenApiSchema GetSchema(bool nullable, IOpenApiAny? example = null, string? description = null)
        =>
        new()
        {
            Nullable = nullable,
            Type = "string",
            Format = "byte",
            Example = example ?? new OpenApiString("U3dhZ2dlciByb2Nrcw=="),
            Description = description
        };

    private readonly string? value;

    public Base64String(string value)
        =>
        this.value = string.IsNullOrEmpty(value) ? null : value;

    public string Value
        =>
        value ?? string.Empty;

    public bool IsValid
        =>
        Convert.TryFromBase64String(
            Value,
            new Span<byte>(new byte[Value.Length]),
            out var _);

    public override string ToString()
        =>
        Value;
}