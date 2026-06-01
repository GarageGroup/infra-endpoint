using System.Collections.Generic;

namespace GarageGroup.Infra.Endpoint;

public interface IEndpointSetMetadataProvider
{
    static abstract IReadOnlyCollection<EndpointMetadata> Metadata { get; }
}