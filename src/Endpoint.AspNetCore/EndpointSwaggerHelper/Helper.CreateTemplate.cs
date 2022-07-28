using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointSwaggerHelper
{
    private const string DefaultDocumentTitle = "HTTP Endpoints API";

    private const string DefaultDocumentVersion = "v1";

    public static OpenApiDocument CreateSwaggerTemplate([AllowNull] this IEnumerable<EndpointMetadata> metadata)
        =>
        new OpenApiDocument
        {
            Info = new()
            {
                Title = DefaultDocumentTitle,
                Version = DefaultDocumentVersion
            }
        }
        .ConfigureEndpoints(metadata);

    private static OpenApiDocument ConfigureEndpoints(this OpenApiDocument document, [AllowNull] IEnumerable<EndpointMetadata> metadata)
    {
        if (metadata is null)
        {
            return document;
        }

        var schemas = new Dictionary<string, OpenApiSchema>(StringComparer.InvariantCultureIgnoreCase);
        var paths = document.Paths ?? new OpenApiPaths();

        foreach (var endpoint in metadata)
        {
            schemas.AddSchemas(endpoint);
            paths.AddPaths(endpoint);
        }

        if (schemas.Count > 0)
        {
            document.Components ??= new();
            document.Components.Schemas = schemas;
        }

        if (paths.Count > 0)
        {
            document.Paths = paths;
        }

        return document;
    }

    private static void AddSchemas(this Dictionary<string, OpenApiSchema> schemas, EndpointMetadata endpoint)
    {
        foreach (var schema in endpoint.Schemas)
        {
            if (schemas.ContainsKey(schema.Key) is false)
            {
                schemas.Add(schema.Key, schema.Value);
            }
        }
    }

    private static void AddPaths(this Dictionary<string, OpenApiPathItem> paths, EndpointMetadata metadata)
    {
        var pathItem = paths.GetOrCreatePathItem(metadata);

        var operationType = metadata.Method.ToOperationType();
        if (pathItem.Operations?.ContainsKey(operationType) is true)
        {
            return;
        }

        if (pathItem.Operations is not null)
        {
            pathItem.Operations.Add(operationType, metadata.Operation);
            return;
        }

        pathItem.Operations = new Dictionary<OperationType, OpenApiOperation>
        {
            [operationType] = metadata.Operation
        };
    }

    private static OpenApiPathItem GetOrCreatePathItem(this Dictionary<string, OpenApiPathItem> paths, EndpointMetadata metadata)
    {
        if (paths.TryGetValue(metadata.Route, out var pathItem))
        {
            return pathItem;
        }

        var createdItem = new OpenApiPathItem
        {
            Summary = metadata.Summary,
            Description = metadata.Description
        };

        paths.Add(metadata.Route, createdItem);
        return createdItem;
    }

    private static OperationType ToOperationType(this EndpointMethod method)
        =>
        method switch
        {
            EndpointMethod.Get => OperationType.Get,
            EndpointMethod.Post => OperationType.Post,
            EndpointMethod.Put => OperationType.Put,
            EndpointMethod.Delete => OperationType.Delete,
            EndpointMethod.Options => OperationType.Options,
            EndpointMethod.Head => OperationType.Head,
            EndpointMethod.Patch => OperationType.Patch,
            EndpointMethod.Trace => OperationType.Trace,
            _ => OperationType.Post
        };
}