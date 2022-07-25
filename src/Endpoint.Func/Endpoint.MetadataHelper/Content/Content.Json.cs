using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static IDictionary<string, OpenApiMediaType> CreateJsonContent(this OpenApiSchema schema)
        =>
        new Dictionary<string, OpenApiMediaType>
        {
            [EndpointContentType.ApplciationJson] = new()
            {
                Schema = schema
            }
        };
}