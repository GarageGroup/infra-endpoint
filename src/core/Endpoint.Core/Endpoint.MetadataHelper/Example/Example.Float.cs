using System.Text.Json.Nodes;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static JsonNode CreateFloatExample(float value)
        =>
        JsonValue.Create(value)!;
}
