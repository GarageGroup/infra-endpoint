using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateProblemSchema()
        =>
        new()
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = CreateStringSchema(true),
                ["title"] = CreateStringSchema(true),
                ["status"] = CreateInt32Schema(true),
                ["detail"] = CreateStringSchema(true),
                ["instance"] = CreateStringSchema(true)
            }
        };
}