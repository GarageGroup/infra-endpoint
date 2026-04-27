using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static IDictionary<string, IOpenApiMediaType> CreateContent(this IOpenApiSchema schema, [AllowNull] string contentType)
        =>
        new Dictionary<string, IOpenApiMediaType>
        {
            [string.IsNullOrEmpty(contentType) ? MediaTypeNames.Text.Plain : contentType] = new OpenApiMediaType()
            {
                Schema = schema
            }
        };
}
