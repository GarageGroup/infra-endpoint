using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointSwaggerConfigurator
{
    internal static void Configure(this EndpointMetadata metadata, OpenApiDocument document)
    {
        if (document is null || metadata is null)
        {
            return;
        }

        if (metadata.Operation.Tags?.Count > 0)
        {
            document.Tags ??= new HashSet<OpenApiTag>();
            document.Tags = document.Tags.InsertTags(metadata.Operation.Tags);
        }

        var paths = document.Paths ?? [];
        paths = paths.InsertPaths(metadata);

        if (paths.Count > 0)
        {
            document.Paths = paths;
        }

        document.Components ??= new();

        var currentSchemas = document.Components.Schemas ?? Enumerable.Empty<KeyValuePair<string, IOpenApiSchema>>();
        var schemas = new Dictionary<string, IOpenApiSchema>(currentSchemas, StringComparer.InvariantCultureIgnoreCase);

        schemas.AddSchemas(metadata);
        document.Components.Schemas = schemas;
    }

    private static ISet<OpenApiTag> InsertTags(this ISet<OpenApiTag> documentTags, IEnumerable<OpenApiTagReference> tags)
    {
        if (tags.Any() is false)
        {
            return documentTags;
        }

        var tagsDictionary = new Dictionary<string, OpenApiTag>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var tag in tags.Reverse().Select(CreateTag).Concat(documentTags))
        {
            var key = GetTagKey(tag);
            if (tagsDictionary.ContainsKey(key))
            {
                continue;
            }

            tagsDictionary[key] = tag;
        }

        return tagsDictionary.Values.ToHashSet();

        static OpenApiTag CreateTag(OpenApiTagReference tag)
            =>
            new()
            {
                Name = tag.Name,
                Description = tag.Description
            };

        static string GetTagKey(OpenApiTag tag)
            =>
            tag.Name ?? string.Empty;
    }

    private static void AddSchemas(this Dictionary<string, IOpenApiSchema> schemas, EndpointMetadata endpoint)
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

        pathItem.Operations = new Dictionary<HttpMethod, OpenApiOperation>
        {
            [operationType] = metadata.Operation
        };

        return paths;
    }

    private static (OpenApiPaths Paths, OpenApiPathItem Item) GetOrCreatePathItem(this OpenApiPaths paths, EndpointMetadata metadata)
    {
        if (paths.TryGetValue(metadata.Route, out var pathItem))
        {
            if (pathItem is OpenApiPathItem openApiPathItem)
            {
                return (paths, openApiPathItem);
            }

            var replacementItem = new OpenApiPathItem
            {
                Operations = pathItem.Operations
            };

            paths[metadata.Route] = replacementItem;
            return (paths, replacementItem);
        }

        var createdItem = new OpenApiPathItem
        {
            Summary = metadata.Summary,
            Description = metadata.Description
        };

        paths.Add(metadata.Route, createdItem);
        return (paths, createdItem);
    }

    private static HttpMethod ToOperationType(this EndpointMethod method)
        =>
        method switch
        {
            EndpointMethod.Get => HttpMethod.Get,
            EndpointMethod.Post => HttpMethod.Post,
            EndpointMethod.Put => HttpMethod.Put,
            EndpointMethod.Delete => HttpMethod.Delete,
            EndpointMethod.Options => HttpMethod.Options,
            EndpointMethod.Head => HttpMethod.Head,
            EndpointMethod.Patch => HttpMethod.Patch,
            EndpointMethod.Trace => HttpMethod.Trace,
            _ => HttpMethod.Post
        };
}
