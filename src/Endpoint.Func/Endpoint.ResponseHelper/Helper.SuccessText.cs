using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GGroupp.Infra.Endpoint;

using IKeyValueCollection = IReadOnlyCollection<KeyValuePair<string, string?>>;

partial class EndpointResponseHelper
{
    public static EndpointResponse ToSuccessTextResponse<T>(
        this T? body, SuccessStatusCode statusCode, [AllowNull] IKeyValueCollection headers)
        =>
        new(
            statusCode: (int)statusCode,
            headers: headers?.Count is not > 0 ? successTextHeaders : headers.Concat(successTextHeaders).ToArray(),
            body: body?.ToTextStream());
}