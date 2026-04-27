using System.Text.Json.Nodes;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static JsonNode CreateBooleanExample(bool value)
        =>
        JsonValue.Create(value)!;
}
