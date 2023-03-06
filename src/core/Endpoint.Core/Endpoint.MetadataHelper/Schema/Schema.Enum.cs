using System;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateEnumSchema<T>(bool nullable, IOpenApiAny? example = null, string? description = null)
        where T : struct, Enum
    {
        return new()
        {
            Type = "string",
            Enum = Enum.GetNames<T>().Select(ToOpenApiString).ToArray(),
            Nullable = nullable,
            Example = example,
            Description = description
        };

        static OpenApiString ToOpenApiString(string value)
            =>
            new(value);
    }
}