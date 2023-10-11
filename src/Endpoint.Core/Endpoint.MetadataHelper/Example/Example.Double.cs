using Microsoft.OpenApi.Any;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiDouble CreateDoubleExample(double value)
        =>
        new(value);
}