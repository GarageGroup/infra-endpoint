using System.Collections.Generic;

namespace GGroupp.Infra.Endpoint;

using IKeyValueCollection = IReadOnlyCollection<KeyValuePair<string, string?>>;

public static partial class EndpointResponseHelper
{
    private const int BadRequestStatusCode = 400;

    private const string ContentTypeHeaderName = "Content-Type";

    private const string ResponseEncoding = "charset=utf-8";

    private const string ProblemJsonContentType = "application/problem+json";

    private static readonly IKeyValueCollection problemJsonHeaders;

    static EndpointResponseHelper()
        =>
        problemJsonHeaders = new KeyValuePair<string, string?>[]
        {
            new(ContentTypeHeaderName, ProblemJsonContentType),
            new(ContentTypeHeaderName, ResponseEncoding)
        };
}