using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static async ValueTask<Result<JsonDocument?, Failure<Unit>>> ParseDocumentAsync(
        this EndpointRequest? request, ILogger? logger, CancellationToken cancellationToken)
    {
        if (request?.Body is null)
        {
            return null;
        }

        try
        {
            return await JsonDocument.ParseAsync(request.Body, default, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger?.LogError(exception, "An unexpected error occured when the request body was being deserialized");
            return Failure.Create("Request body is incorrect");
        }
    }
}