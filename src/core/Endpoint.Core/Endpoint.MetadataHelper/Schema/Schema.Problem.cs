using System.Collections.Generic;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateProblemSchema()
        =>
        new()
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = CreateStringSchema(true),
                ["title"] = CreateStringSchema(true),
                ["status"] = CreateInt32Schema(true),
                ["detail"] = CreateStringSchema(true),
                ["instance"] = CreateStringSchema(true)
            }
        };
}
