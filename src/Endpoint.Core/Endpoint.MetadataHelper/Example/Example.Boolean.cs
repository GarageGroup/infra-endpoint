using Microsoft.OpenApi.Any;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiBoolean CreateBooleanExample(bool value)
        =>
        new(value);
}