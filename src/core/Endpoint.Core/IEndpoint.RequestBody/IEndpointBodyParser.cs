using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Endpoint;

namespace GarageGroup.Infra;

public interface IEndpointBodyParser<T>
{
    static abstract ValueTask<Result<T, Failure<Unit>>> ParseAsync(EndpointRequest request, CancellationToken cancellationToken);
}