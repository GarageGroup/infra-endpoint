using Microsoft.OpenApi.Any;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiInteger CreateIntegerExample(int value)
        =>
        new(value);
}