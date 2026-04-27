using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

public static partial class EndpointMetadataHelper
{
    private const string ProblemJsonContentType = "application/problem+json";

    private static JsonSchemaType CreateSchemaType(JsonSchemaType type, bool nullable)
        =>
        nullable ? type | JsonSchemaType.Null : type;
}
