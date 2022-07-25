using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

using IKeyValueCollection = IReadOnlyCollection<KeyValuePair<string, string?>>;

partial class EndpointResponseHelper
{
    public static EndpointResponse ToSuccessJsonResponse<T>(
        this T? body, SuccessStatusCode statusCode, [AllowNull] IKeyValueCollection headers, JsonSerializerOptions? jsonSerializerOptions)
        =>
        new(
            statusCode: (int)statusCode,
            headers: headers?.Count is not > 0 ? successJsonHeaders : headers.Concat(successJsonHeaders).ToArray(),
            body: body?.SerializeToStream(jsonSerializerOptions));
}