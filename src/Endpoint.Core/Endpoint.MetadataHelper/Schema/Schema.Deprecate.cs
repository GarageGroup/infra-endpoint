using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    [return: NotNullIfNotNull(nameof(schema))]
    public static TOpenApiSchema? Deprecate<TOpenApiSchema>(this TOpenApiSchema schema)
        where TOpenApiSchema : OpenApiSchema
    {
        if (schema is null)
        {
            return null;
        }

        schema.Deprecated = true;
        return schema;
    }
}