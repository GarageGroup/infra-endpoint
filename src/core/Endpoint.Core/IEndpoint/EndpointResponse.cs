using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace GarageGroup.Infra.Endpoint;

using IKeyValueCollection = IReadOnlyCollection<KeyValuePair<string, string?>>;

public sealed record EndpointResponse
{
    public EndpointResponse(int statusCode, [AllowNull] IKeyValueCollection headers, Stream? body)
    {
        StatusCode = statusCode;
        Headers = headers ?? Array.Empty<KeyValuePair<string, string?>>();
        Body = body;
    }

    public int StatusCode { get; }

    public IKeyValueCollection Headers { get; }

    public Stream? Body { get; }
}