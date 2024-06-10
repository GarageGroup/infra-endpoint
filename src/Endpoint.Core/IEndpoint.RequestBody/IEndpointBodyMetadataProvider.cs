using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

public interface IEndpointBodyMetadataProvider
{
    static abstract OpenApiRequestBody GetEndpointBodyMetadata();
}