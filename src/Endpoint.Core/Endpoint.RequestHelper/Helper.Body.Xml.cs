using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static async ValueTask<Result<T, Failure<Unit>>> DeserializeXmlBodyAsync<T>(
        this EndpointRequest? request, CancellationToken cancellationToken)
    {
        if (request?.Body is null)
        {
            return default(T)!;
        }

        try
        {
            using var stream = new MemoryStream();
            await request.Body.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);

            stream.Position = 0;

            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream)!;
        }
        catch (Exception exception)
        {
            return exception.ToFailure("Failed to deserialize the request body into XML");
        }
    }
}