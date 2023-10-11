using Microsoft.OpenApi.Any;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiFloat CreateFloatExample(float value)
        =>
        new(value);
}