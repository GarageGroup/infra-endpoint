namespace GGroupp.Infra.Endpoint;

public interface IEndpointMetadataProvider
{
    static abstract EndpointMetadata GetEndpointMetadata();
}