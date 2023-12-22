using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static ValueTask<string> ReadStringAsync(this EndpointRequest? request, CancellationToken cancellationToken)
        =>
        request.InnerReadStringAsync(cancellationToken);

    private static async ValueTask<string> InnerReadStringAsync(this EndpointRequest? request, CancellationToken cancellationToken)
    {
        if (request?.Body is null)
        {
            return string.Empty;
        }

        using var reader = new StreamReader(request.Body);
        return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    }
}