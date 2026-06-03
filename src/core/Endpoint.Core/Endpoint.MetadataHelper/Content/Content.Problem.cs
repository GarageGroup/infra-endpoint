using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static IDictionary<string, IOpenApiMediaType> CreateProblemContent()
        =>
        CreateProblemContent([]);

    public static IDictionary<string, IOpenApiMediaType> CreateProblemContent(
        params KeyValuePair<string, JsonNode>[] examples)
    {
        var mediaType = new OpenApiMediaType()
        {
            Schema = CreateProblemSchema()
        };

        if (examples.Length is 1)
        {
            mediaType.Example = examples[0].Value;
        }
        else if (examples.Length > 1)
        {
            mediaType.Examples = new Dictionary<string, IOpenApiExample>(examples.Length);

            foreach (var example in examples)
            {
                mediaType.Examples[example.Key] = new OpenApiExample()
                {
                    Value = example.Value
                };
            }
        }

        return new Dictionary<string, IOpenApiMediaType>
        {
            [ProblemJsonContentType] = mediaType
        };
    }

    public static JsonObject CreateProblemExample(string? type, string? title, int status, string? detail)
        =>
        new()
        {
            ["type"] = string.IsNullOrEmpty(type) ? null : JsonValue.Create(type),
            ["title"] = string.IsNullOrEmpty(title) ? JsonValue.Create("about:blank") : JsonValue.Create(title),
            ["status"] = JsonValue.Create(status),
            ["detail"] = string.IsNullOrEmpty(detail) ? null : JsonValue.Create(detail)
        };
}