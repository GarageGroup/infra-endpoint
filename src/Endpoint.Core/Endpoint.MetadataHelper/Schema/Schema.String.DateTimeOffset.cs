using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDateTimeOffsetSchema(bool nullable, IOpenApiAny? example = null, string? description = null)
        =>
        new()
        {
            Type = "string",
            Format = "date-time",
            Nullable = nullable,
            Example = example ?? new OpenApiString(DateTimeOffset.Now.ToString("o")),
            Description = description
        };
}