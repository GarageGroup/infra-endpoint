using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra.Endpoint;

public interface IEndpoint : IEndpointMetadataProvider
{
    Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default);
}