using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static async ValueTask<Result<NameValueCollection, Failure<Unit>>> ReadFormDataAsync(
        this EndpointRequest? request, CancellationToken cancellationToken)
    {
        var text = await request.InnerReadStringAsync(cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(text))
        {
            return Result.Success<NameValueCollection>([]);
        }

        try
        {
            return HttpUtility.ParseQueryString(text);
        }
        catch (Exception exception)
        {
            return exception.ToFailure("Failed to parse the request body form parameters");
        }
    }
}