using System;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDateTimeSchema(bool nullable, JsonNode? example = null, string? description = null)
        =>
        new()
        {
            Type = CreateSchemaType(JsonSchemaType.String, nullable),
            Format = "date-time",
            Example = example ?? JsonValue.Create(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
            Description = description
        };
}