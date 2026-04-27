using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static ValueTask<Result<JsonDocument?, Failure<Unit>>> ParseDocumentAsync(
        this EndpointRequest? request, CancellationToken cancellationToken)
        =>
        request.ParseDocumentAsync(null, cancellationToken);

    public static async ValueTask<Result<JsonDocument?, Failure<Unit>>> ParseDocumentAsync(
        this EndpointRequest? request, ILogger? logger, CancellationToken cancellationToken)
    {
        if (request?.Body is null)
        {
            return null;
        }

        string? body = null;
        try
        {
            body = await request.InnerReadStringAsync(cancellationToken).ConfigureAwait(false);
            return JsonDocument.Parse(body, DocumentOptions);
        }
        catch (Exception exception)
        {
            logger?.LogDebug(exception, "Failed to deserialize the request body into JSON. Request body: '{body}'", body);
            return Failure.Create("Failed to deserialize the request body into JSON");
        }
    }
}