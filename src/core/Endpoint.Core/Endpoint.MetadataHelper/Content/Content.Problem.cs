using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra.Endpoint;

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