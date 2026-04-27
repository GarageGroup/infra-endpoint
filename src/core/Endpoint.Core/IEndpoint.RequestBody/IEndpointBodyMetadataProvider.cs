using Microsoft.OpenApi;

namespace GarageGroup.Infra;

public interface IEndpointBodyMetadataProvider
{
    static abstract OpenApiRequestBody GetEndpointBodyMetadata();
}