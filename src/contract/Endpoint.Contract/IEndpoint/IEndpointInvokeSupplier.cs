using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra.Endpoint;

public interface IEndpointInvokeSupplier
{
    Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default);
}