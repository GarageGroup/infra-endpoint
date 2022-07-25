using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

using IKeyValueCollection = IReadOnlyCollection<KeyValuePair<string, string?>>;

public static partial class EndpointResponseHelper
{
    private const string ContentTypeHeaderName = "Content-Type";

    private const string ResponseEncoding = "charset=utf-8";

    private static readonly IKeyValueCollection problemJsonHeaders;

    private static readonly IKeyValueCollection successJsonHeaders;

    private static readonly IKeyValueCollection successTextHeaders;

    static EndpointResponseHelper()
    {
        problemJsonHeaders = new KeyValuePair<string, string?>[]
        {
            new(ContentTypeHeaderName, EndpointContentType.ProblemJson),
            new(ContentTypeHeaderName, ResponseEncoding)
        };

        successJsonHeaders = new KeyValuePair<string, string?>[]
        {
            new(ContentTypeHeaderName, EndpointContentType.ApplciationJson),
            new(ContentTypeHeaderName, ResponseEncoding)
        };

        successTextHeaders = new KeyValuePair<string, string?>[]
        {
            new(ContentTypeHeaderName, EndpointContentType.PlainText),
            new(ContentTypeHeaderName, ResponseEncoding)
        };
    }

    private static Stream SerializeToStream<T>(this T value, JsonSerializerOptions? jsonSerializerOptions)
    {
        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, value, jsonSerializerOptions);

        stream.Position = 0;
        return stream;
    }

    private static Stream? ToTextStream<T>(this T value)
    {
        var text = value?.ToString();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }
}