using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static IDictionary<string, OpenApiMediaType> CreateProblemContent()
        =>
        new Dictionary<string, OpenApiMediaType>
        {
            [ProblemJsonContentType] = new()
            {
                Schema = CreateReferenceSchema(false, "ProblemDetails")
            }
        };
}