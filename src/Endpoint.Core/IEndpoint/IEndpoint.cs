using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra.Endpoint;

public interface IEndpoint : IEndpointMetadataProvider
{
    Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default);
}