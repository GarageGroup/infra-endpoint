using System.Collections.Generic;

namespace GarageGroup.Infra.Endpoint;

public static partial class EndpointResponseHelper
{
    private const int BadRequestStatusCode = 400;

    private const string ContentTypeHeaderName = "Content-Type";

    private const string ResponseEncoding = "charset=utf-8";

    private const string ProblemJsonContentType = "application/problem+json";

    private static readonly IReadOnlyCollection<KeyValuePair<string, string?>> problemJsonHeaders;

    static EndpointResponseHelper()
        =>
        problemJsonHeaders =
        [
            new(ContentTypeHeaderName, ProblemJsonContentType),
            new(ContentTypeHeaderName, ResponseEncoding)
        ];
}