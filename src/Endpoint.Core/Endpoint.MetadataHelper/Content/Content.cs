using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static IDictionary<string, OpenApiMediaType> CreateContent(this OpenApiSchema schema, [AllowNull] string contentType)
        =>
        new Dictionary<string, OpenApiMediaType>
        {
            [contentType ?? string.Empty] = new()
            {
                Schema = schema
            }
        };
}