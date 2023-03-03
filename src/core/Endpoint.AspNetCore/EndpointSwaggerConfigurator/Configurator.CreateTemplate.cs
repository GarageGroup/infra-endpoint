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

#if NET7_0_OR_GREATER
        var metadata = TEndpoint.GetEndpointMetadata();
#else
        var metadata = IEndpointMetadataProvider.GetEndpointMetadata<TEndpoint>();
#endif

        if (metadata is null)
        {
            return;
        }

        if (metadata.Operation.Tags?.Count > 0)
        {
            document.Tags ??= new List<OpenApiTag>();
            document.Tags.AddTags(metadata.Operation.Tags);
        }

        var paths = document.Paths ?? new OpenApiPaths();
        paths = paths.InsertPaths(metadata);

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

    private static void AddTags(this IList<OpenApiTag> documentTags, IEnumerable<OpenApiTag> tags)
    {
        foreach (var tag in tags.Where(NotExisted))
        {
            documentTags.Add(tag);
        }

        bool NotExisted(OpenApiTag tag)
            =>
            documentTags.Any(t => string.Equals(t.Name, tag.Name, StringComparison.InvariantCultureIgnoreCase)) is false;
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

    private static OpenApiPaths InsertPaths(this OpenApiPaths source, EndpointMetadata metadata)
    {
        var (paths, pathItem) = source.GetOrCreatePathItem(metadata);

        var operationType = metadata.Method.ToOperationType();
        if (pathItem.Operations?.ContainsKey(operationType) is true)
        {
            return paths;
        }

        if (pathItem.Operations is not null)
        {
            pathItem.Operations = pathItem.Operations.ToDictionary().Insert(operationType, metadata.Operation);
            return paths;
        }

        pathItem.Operations = new Dictionary<OperationType, OpenApiOperation>
        {
            [operationType] = metadata.Operation
        };

        return paths;
    }

    private static (OpenApiPaths Paths, OpenApiPathItem Item) GetOrCreatePathItem(this OpenApiPaths paths, EndpointMetadata metadata)
    {
        if (paths.TryGetValue(metadata.Route, out var pathItem))
        {
            return (paths, pathItem);
        }

        var createdItem = new OpenApiPathItem
        {
            Summary = metadata.Summary,
            Description = metadata.Description
        };

        return(paths.Insert(metadata.Route, createdItem), createdItem);
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