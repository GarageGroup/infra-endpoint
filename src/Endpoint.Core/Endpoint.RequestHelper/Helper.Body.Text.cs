using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static async ValueTask<string> ReadStringAsync(this EndpointRequest? request, CancellationToken _)
    {
        if (request?.Body is null)
        {
            return string.Empty;
        }

        using var reader = new StreamReader(request.Body);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }
}