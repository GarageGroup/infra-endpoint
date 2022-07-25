using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static IDictionary<string, OpenApiMediaType> CreateTextContent(this OpenApiSchema schema)
        =>
        new Dictionary<string, OpenApiMediaType>
        {
            [EndpointContentType.PlainText] = new()
            {
                Schema = schema
            }
        };
}