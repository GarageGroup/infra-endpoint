using System.Collections.Generic;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static IDictionary<string, IOpenApiMediaType> CreateProblemContent()
        =>
        new Dictionary<string, IOpenApiMediaType>
        {
            [ProblemJsonContentType] = new OpenApiMediaType()
            {
                Schema = CreateReferenceSchema(false, "ProblemDetails")
            }
        };
}
