using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static async ValueTask<Result<JsonDocument?, Failure<Unit>>> ParseDocumentAsync(
        this EndpointRequest? request, CancellationToken cancellationToken)
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
            return exception.ToFailure("Failed to deserialize the request body into JSON");
        }
    }
}