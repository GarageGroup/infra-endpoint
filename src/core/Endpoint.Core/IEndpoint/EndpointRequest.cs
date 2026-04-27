using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Claims;

namespace GarageGroup.Infra.Endpoint;

using IKeyValueCollection = IReadOnlyCollection<KeyValuePair<string, string?>>;

public sealed record class EndpointRequest
{
    private static readonly ClaimsPrincipal EmptyPrincipal;

    static EndpointRequest()
        =>
        EmptyPrincipal = new();

    public EndpointRequest(
        [AllowNull] IKeyValueCollection headers,
        [AllowNull] IKeyValueCollection queryParameters,
        [AllowNull] IKeyValueCollection routeValues,
        [AllowNull] ClaimsPrincipal user,
        Stream? body)
    {
        Headers = headers ?? [];
        QueryParameters = queryParameters ?? [];
        RouteValues = routeValues ?? [];
        User = user ?? EmptyPrincipal;
        Body = body;
    }

    public IKeyValueCollection Headers { get; }

    public IKeyValueCollection QueryParameters { get; }

    public IKeyValueCollection RouteValues { get; }

    public ClaimsPrincipal User { get; }

    public Stream? Body { get; }
}