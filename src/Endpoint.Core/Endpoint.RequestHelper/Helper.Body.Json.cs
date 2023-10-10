using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static async ValueTask<Result<T, Failure<Unit>>> DeserializeBodyAsync<T>(
        this EndpointRequest? request, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken)
    {
        if (request?.Body is null)
        {
            return default(T)!;
        }

        try
        {
            return (await JsonSerializer.DeserializeAsync<T>(request.Body, jsonSerializerOptions, cancellationToken).ConfigureAwait(false))!;
        }
        catch (Exception exception)
        {
            return exception.ToFailure("Failed to deserialize the request body into JSON");
        }
    }
}