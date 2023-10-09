using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static IDictionary<string, OpenApiMediaType> CreateContent(this OpenApiSchema schema, [AllowNull] string contentType)
        =>
        new Dictionary<string, OpenApiMediaType>
        {
            [string.IsNullOrEmpty(contentType) ? "text/plain" : contentType] = new()
            {
                Schema = schema
            }
        };
}