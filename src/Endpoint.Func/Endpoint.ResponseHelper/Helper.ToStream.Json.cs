using System.IO;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointResponseHelper
{
    public static Stream? ToJsonStream<T>(this T? value, JsonSerializerOptions? jsonSerializerOptions)
    {
        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, value, jsonSerializerOptions);

        stream.Position = 0;
        return stream;
    }
}