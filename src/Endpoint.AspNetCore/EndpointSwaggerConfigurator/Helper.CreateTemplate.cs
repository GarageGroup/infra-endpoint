using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointSwaggerConfigurator
{
    internal static void Configure<TEndpoint>(OpenApiDocument document)
        where TEndpoint : class, IEndpointMetadataProvider
    {
        if (document is null)
        {
            return;
        }

        var metadata = TEndpoint.GetEndpointMetadata();
        if (metadata is null)
        {
            return;
        }

        var paths = document.Paths ?? new OpenApiPaths();
        paths.AddPaths(metadata);

        if (paths.Count > 0)
        {
            document.Paths = paths;
        }

        document.Components ??= new();

        var currentSchemas = document.Components.Schemas ?? Enumerable.Empty<KeyValuePair<string, OpenApiSchema>>();
        var schemas = new Dictionary<string, OpenApiSchema>(currentSchemas, StringComparer.InvariantCultureIgnoreCase);

        schemas.AddSchemas(metadata);
        document.Components.Schemas = schemas;
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