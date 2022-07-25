using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Claims;

namespace GGroupp.Infra.Endpoint;

using IKeyValueCollection = IReadOnlyCollection<KeyValuePair<string, string?>>;

public sealed record class EndpointRequest
{
    private static readonly IKeyValueCollection EmptyKeyValueCollection;

    private static readonly ClaimsPrincipal EmptyPrincipal;

    static EndpointRequest()
    {
        EmptyKeyValueCollection = Array.Empty<KeyValuePair<string, string?>>();
        EmptyPrincipal = new();
    }

    public EndpointRequest(
        [AllowNull] IKeyValueCollection headers,
        [AllowNull] IKeyValueCollection queryParameters,
        [AllowNull] IKeyValueCollection routeValues,
        [AllowNull] ClaimsPrincipal user,
        Stream? body)
    {
        Headers = headers ?? EmptyKeyValueCollection;
        QueryParameters = queryParameters ?? EmptyKeyValueCollection;
        RouteValues = routeValues ?? EmptyKeyValueCollection;
        User = user ?? EmptyPrincipal;
        Body = body;
    }

    public IKeyValueCollection Headers { get; }

    public IKeyValueCollection QueryParameters { get; }

    public IKeyValueCollection RouteValues { get; }

    public ClaimsPrincipal User { get; }

    public Stream? Body { get; }
}