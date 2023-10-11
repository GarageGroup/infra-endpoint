using Microsoft.OpenApi.Any;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiString CreateStringExample(string? value)
        =>
        new(value);
}