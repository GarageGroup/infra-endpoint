using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

using ISchemaCollection = IReadOnlyCollection<KeyValuePair<string, OpenApiSchema>>;

public sealed record class EndpointMetadata
{
    private static readonly OpenApiOperation EmptyOperation = new();

    public EndpointMetadata(
        EndpointMethod method,
        [AllowNull] string route,
        [AllowNull] string summary,
        [AllowNull] string description,
        [AllowNull] OpenApiOperation operation,
        [AllowNull] ISchemaCollection schemas)
    {
        Method = method;
        Route = route ?? string.Empty;
        Summary = string.IsNullOrEmpty(summary) ? null : summary;
        Description = string.IsNullOrEmpty(description) ? null : description;
        Operation = operation ?? EmptyOperation;
        Schemas = schemas ?? Array.Empty<KeyValuePair<string, OpenApiSchema>>();
    }

    public EndpointMethod Method { get; }

    public string Route { get; }

    public string? Summary { get; }

    public string? Description { get; }

    public OpenApiOperation Operation { get; }

    public ISchemaCollection Schemas { get; }
}