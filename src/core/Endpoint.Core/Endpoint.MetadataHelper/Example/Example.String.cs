using System.Text.Json.Nodes;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static JsonNode? CreateStringExample(string? value)
        =>
        value is null ? null : JsonValue.Create(value);
}
