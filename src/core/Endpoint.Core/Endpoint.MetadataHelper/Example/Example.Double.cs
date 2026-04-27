using System.Text.Json.Nodes;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static JsonNode CreateDoubleExample(double value)
        =>
        JsonValue.Create(value)!;
}
