using System.Text.Json.Nodes;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static JsonNode CreateIntegerExample(int value)
        =>
        JsonValue.Create(value)!;
}
