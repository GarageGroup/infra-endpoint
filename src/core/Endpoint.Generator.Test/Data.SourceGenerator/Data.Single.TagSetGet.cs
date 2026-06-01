namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string TagSetGetSourceCode
        =
        """
        using System;
        using System.Collections.Generic;
        using System.Threading;
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo
        {
            using static TagSetGetMetadata;

            [Endpoint(Func.OperationId, EndpointMethod.Post, Func.Route, Summary = Func.Summary, Description = Func.Description)]
            public sealed class TagSetGetFunc
            {
                public Task<Result<TagSetGetOut, Failure<Unit>>> InvokeAsync(
                    TagSetGetIn input, CancellationToken cancellationToken)
                    =>
                    default!;
            }

            public sealed record class TagSetGetIn
            {
                public TagSetGetIn(
                    [ClaimIn("oid")] Guid systemUserId,
                    [JsonBodyIn, SwaggerDescription(In.ProjectIdDescription), StringExample(In.ProjectIdExample)] Guid projectId)
                {
                    SystemUserId = systemUserId;
                    ProjectId = projectId;
                }

                public Guid SystemUserId { get; }

                public Guid ProjectId { get; }
            }

            public readonly record struct TagSetGetOut
            {
                [JsonBodyOut]
                [SwaggerDescription(Out.TagsDescription)]
                [StringExample(Out.TagExample)]
                public required FlatArray<string> Tags { get; init; }
            }

            internal static class TagSetGetMetadata
            {
                internal static class Func
                {
                    public const string OperationId = "TagSetGet";

                    public const string Tag = "Tag";

                    public const string Route = "/getTags";

                    public const string Summary = "Get tags";

                    public const string Description = "Retrieves a list of tags associated with user's timesheets.";
                }

                internal static class In
                {
                    public const string ProjectIdDescription = "Unique identifier of the project in Dataverse.";

                    public const string ProjectIdExample = "9dfb0e67-e565-4787-bab6-d92a2cf6bb70";
                }

                internal static class Out
                {
                    public const string TagsDescription = "Array of tags associated with user's timesheets.";

                    public const string TagExample = "#Task8137";
                }
            }
        }

        namespace GarageGroup.Infra
        {
            public readonly record struct FlatArray<T>(IReadOnlyList<T> Items);
        }

        namespace System
        {
            public readonly record struct Unit;

            public readonly record struct Failure<TFailureCode>(TFailureCode FailureCode, Exception? SourceException, string FailureMessage);

            public readonly record struct Result<TSuccess, TFailure>(TSuccess Success, TFailure Failure)
            {
                public bool IsFailure
                    =>
                    false;

                public TSuccess SuccessOrThrow()
                    =>
                    Success;

                public TFailure FailureOrThrow()
                    =>
                    Failure;

                public TOutput Fold<TOutput>(Func<TSuccess, TOutput> onSuccess, Func<TFailure, TOutput> onFailure)
                    =>
                    onSuccess(Success);
            }
        }
        """;

    internal const string TagSetGetFactoryHintName
        =
        "TagSetGetEndpoint.g.cs";

    internal const string TagSetGetInvokeHintName
        =
        "TagSetGetEndpoint.Invoke.g.cs";

    internal const string TagSetGetMetadataHintName
        =
        "TagSetGetEndpoint.Metadata.g.cs";

    internal const string TagSetGetFactorySourceCode
        =
        """
        // Auto-generated code by PrimeFuncPack
        #nullable enable

        using GarageGroup.Infra.Endpoint;
        using Microsoft.Extensions.Logging;
        using System;
        using System.Text.Json;

        namespace Demo;

        [EndpointMetadata("POST", "/getTags")]
        [EndpointOperationMetadata("TagSetGet", "POST", "/getTags")]
        public sealed partial class TagSetGetEndpoint : IEndpoint
        {
            public static TagSetGetEndpoint Resolve(IServiceProvider? serviceProvider, TagSetGetFunc endpointFunc)
                =>
                new(
                    endpointFunc: endpointFunc ?? throw new ArgumentNullException(nameof(endpointFunc)),
                    logger: serviceProvider?.GetEndpointLogger<TagSetGetEndpoint>());

            private static readonly JsonSerializerOptions SerializerOptions = EndpointDeserializer.CreateDeafultOptions();

            private readonly TagSetGetFunc endpointFunc;

            private readonly ILogger? logger;

            private TagSetGetEndpoint(TagSetGetFunc endpointFunc, ILogger? logger)
            {
                this.endpointFunc = endpointFunc;
                this.logger = logger;
            }
        }
        """;

    internal const string TagSetGetInvokeSourceCode
        =
        """
        // Auto-generated code by PrimeFuncPack
        #nullable enable

        using GarageGroup.Infra;
        using GarageGroup.Infra.Endpoint;
        using Microsoft.Extensions.Logging;
        using System;
        using System.Collections.Generic;
        using System.IO;
        using System.Text.Json;
        using System.Threading;
        using System.Threading.Tasks;

        namespace Demo;

        partial class TagSetGetEndpoint
        {
            public async Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
            {
                var inputResult = await MapRequestAsync(request, cancellationToken).ConfigureAwait(false);
                if (inputResult.IsFailure)
                {
                    var inputFailure = inputResult.FailureOrThrow();

                    logger?.LogError(inputFailure.SourceException, "Request is incorrect: {failureMessage}", inputFailure.FailureMessage);
                    return inputResult.FailureOrThrow().ToBadRequestResponse(SerializerOptions);
                }

                var input = inputResult.SuccessOrThrow();
                var endpointResult = await endpointFunc.InvokeAsync(input, cancellationToken).ConfigureAwait(false);

                return endpointResult.Fold(MapSuccess, MapFailure);
            }

            private async ValueTask<Result<TagSetGetIn, Failure<Unit>>> MapRequestAsync(EndpointRequest request, CancellationToken token)
            {
                var systemUserIdResult = EndpointParser.ParseGuid(request.GetClaimValue("oid"));
                if (systemUserIdResult.IsFailure)
                {
                    return systemUserIdResult.FailureOrThrow();
                }

                var bodyDocumentResult = await request.ParseDocumentAsync(logger, token).ConfigureAwait(false);
                if (bodyDocumentResult.IsFailure)
                {
                    return bodyDocumentResult.FailureOrThrow();
                }

                var bodyDocument = bodyDocumentResult.SuccessOrThrow();

                var projectIdResult = bodyDocument.GetGuidOrFailure("projectId");
                if (projectIdResult.IsFailure)
                {
                    return projectIdResult.FailureOrThrow();
                }

                return new TagSetGetIn(
                    systemUserId: systemUserIdResult.SuccessOrThrow(),
                    projectId: projectIdResult.SuccessOrThrow());
            }

            private EndpointResponse MapSuccess(TagSetGetOut success)
            {
                return new(
                    statusCode: 200,
                    headers: new KeyValuePair<string, string?>[]
                    {
                        new("Content-Type", "application/json; charset=utf-8")
                    },
                    body: InnerGetBody());

                Stream InnerGetBody()
                {
                    var stream = new MemoryStream();
                    using var writer = new Utf8JsonWriter(stream);

                    writer.WriteStartObject();

                    writer.WritePropertyName("tags");
                    JsonSerializer.Serialize(writer, success.Tags, SerializerOptions);

                    writer.WriteEndObject();
                    writer.Flush();

                    stream.Position = 0;
                    return stream;
                }
            }

            private EndpointResponse MapFailure(Failure<Unit> failure)
            {
                logger?.LogError(failure.SourceException, "An unexpected http error: {errorCode}. Message: {message}", failure.FailureCode, failure.FailureMessage);
                return new(500, default, default);
            }
        }
        """;

    internal const string TagSetGetMetadataSourceCode
        =
        """
        // Auto-generated code by PrimeFuncPack
        #nullable enable
        
        using GarageGroup.Infra;
        using GarageGroup.Infra.Endpoint;
        using Microsoft.OpenApi;
        using System.Collections.Generic;
        
        namespace Demo;
        
        using static EndpointMetadataHelper;
        
        partial class TagSetGetEndpoint
        {
            public static EndpointMetadata GetEndpointMetadata()
                =>
                new(
                    method: EndpointMethod.Post,
                    route: "/getTags",
                    summary: default,
                    description: default,
                    operation: new()
                    {
                        OperationId = BuildOperationId("TagSetGet"),
                        Summary = "Get tags",
                        Description = "Retrieves a list of tags associated with user's timesheets.",
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(
                                string.Empty,
                                new()
                                {
                                    Tags = new HashSet<OpenApiTag>
                                    {
                                        new()
                                        {
                                            Name = string.Empty,
                                            Description = default
                                        }
                                    },
                                },
                                null)
                        },
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Object,
                                Properties = new Dictionary<string, IOpenApiSchema>
                                {
                                    ["projectId"] = CreateUuidSchema(false, example: CreateStringExample("9dfb0e67-e565-4787-bab6-d92a2cf6bb70"), description: "Unique identifier of the project in Dataverse."),
                                }
                            }
                            .CreateContent("application/json")
                        },
                        Responses = new()
                        {
                            ["200"] = new OpenApiResponse()
                            {
                                Description = "Success",
                                Content = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.Object,
                                    Properties = new Dictionary<string, IOpenApiSchema>
                                    {
                                        ["tags"] = new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.Object,
                                            Example = CreateStringExample("#Task8137"),
                                            Properties = new Dictionary<string, IOpenApiSchema>
                                            {
                                                ["items"] = new OpenApiSchema
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Items = CreateStringSchema(false, example: default, description: default),
                                                },
                                            }
                                        },
                                    }
                                }
                                .CreateContent("application/json")
                            },
                        }
                    },
                    schemas: new Dictionary<string, IOpenApiSchema>()
                    {
                    })
                {
                    OperationId = "TagSetGet"
                };
        }
        """;
}
