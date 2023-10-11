using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    [return: NotNullIfNotNull(nameof(schema))]
    public static TOpenApiSchema? WithXml<TOpenApiSchema>(this TOpenApiSchema schema, OpenApiXml xml)
        where TOpenApiSchema : OpenApiSchema
    {
        if (schema is null)
        {
            return null;
        }

        schema.Xml = xml;
        return schema;
    }
}