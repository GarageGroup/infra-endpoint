using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static ValueTask<Result<T, Failure<Unit>>> DeserializeBodyAsync<T>(
        this EndpointRequest? request, JsonSerializerOptions? jsonSerializerOptions, CancellationToken cancellationToken)
        =>
        request.DeserializeBodyAsync<T>(jsonSerializerOptions, null, cancellationToken);

    public static async ValueTask<Result<T, Failure<Unit>>> DeserializeBodyAsync<T>(
        this EndpointRequest? request, JsonSerializerOptions? jsonSerializerOptions, ILogger? logger, CancellationToken cancellationToken)
    {
        if (request?.Body is null)
        {
            return default(T)!;
        }

        string? body = null;
        try
        {
            body = await request.InnerReadStringAsync(cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(body, jsonSerializerOptions)!;
        }
        catch (Exception exception)
        {
            logger?.LogDebug(exception, "Failed to deserialize the request body into JSON. Request body: '{body}'", body);
            return Failure.Create("Failed to deserialize the request body into JSON");
        }
    }
}