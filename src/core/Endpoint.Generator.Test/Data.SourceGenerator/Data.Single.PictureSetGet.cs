namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string PictureSetGetSourceCode =
        """
        using System;
        using System.Collections.Generic;
        using System.Threading;
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo
        {
            using static PictureSetGetMetadata;

            [Endpoint(Func.OperationId, EndpointMethod.Get, Func.Route, Summary = Func.Summary, Description = Func.Description)]
            [EndpointTag(Func.TagName, Description = Func.TagDescription)]
            public interface IPictureSetGetFunc
            {
                ValueTask<PictureSetGetOut> InvokeAsync(PictureSetGetIn input, CancellationToken cancellationToken);
            }

            public sealed record class PictureSetGetIn
            {
                public PictureSetGetIn(
                    [RouteIn(Description = Input.EntityDescription)] PictureEntityType entity,
                    [RouteIn(Description = Input.IdDescription)] int id,
                    [QueryIn(Description = Input.TypeDescription)] PictureType? type,
                    [QueryIn(Description = Input.FormatsDescription), StringExample(Input.FormatsExample)] JsonStringValue<FlatArray<string>>? formats)
                {
                    Entity = entity;
                    Id = id;
                    Type = type;
                    Formats = formats?.Value ?? default;
                }

                public PictureEntityType Entity { get; }

                public int Id { get; }

                public PictureType? Type { get; }

                public FlatArray<string> Formats { get; }
            }

            public readonly record struct PictureSetGetOut
            {
                public PictureSetGetOut(FlatArray<PictureOut> pictures)
                    =>
                    Pictures = pictures;

                [RootBodyOut]
                public FlatArray<PictureOut> Pictures { get; }
            }

            public sealed record class PictureOut
            {
                public PictureOut(string url, PictureType? type, string? format, DateOnly? constructionDate = null)
                {
                    Url = url;
                    Type = type;
                    Format = format;
                    ConstructionDate = constructionDate;
                }

                [SwaggerDescription(Output.UrlDescription)]
                public string Url { get; }

                [SwaggerDescription(Output.TypeDescription)]
                public PictureType? Type { get; init; }

                [SwaggerDescription(Output.FormatDescription)]
                public string? Format { get; }

                [SwaggerDescription(Output.ConstructionDateDescription)]
                public DateOnly? ConstructionDate { get; }
            }

            public enum PictureEntityType
            {
                Property,

                Project
            }

            public enum PictureType
            {
                Images = 0,

                Plans = 1,

                Brochures = 3,

                Interiors = 4,

                Outside = 5,

                ConstructionProgress = 6,

                Other = 7,

                Location = 8
            }

            internal static class PictureSetGetMetadata
            {
                internal static class Func
                {
                    public const string OperationId = "PictureSetGet";

                    public const string Route = "/pictures/{entity}/{id}";

                    public const string Summary = "Get pictures";

                    public const string Description = "Returns pictures in required format";

                    public const string TagName = "Detailed information";

                    public const string TagDescription = "Methods that return detailed information about objects";
                }

                internal static class Input
                {
                    public const string EntityDescription = "Entity type";

                    public const string IdDescription = "Entity ID";

                    public const string TypeDescription = "Picture type";

                    public const string FormatsExample = "[\"150x90_60\",\"634x468_60\"]";

                    public const string FormatsDescription = "Picture format(s)";
                }

                internal static class Output
                {
                    public const string UrlDescription = "Picture URL";

                    public const string TypeDescription = "Picture type";

                    public const string FormatDescription = "Picture format";

                    public const string ConstructionDateDescription = "Picture construction date";
                }
            }
        }

        namespace GarageGroup.Infra
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

            public interface IEndpointTypeParser<TSelf>
                where TSelf : IEndpointTypeParser<TSelf>
            {
            }

            public readonly record struct JsonStringValue<TValue>(TValue Value) : IEndpointTypeParser<JsonStringValue<TValue>>
            {
                public static Result<JsonStringValue<TValue>, Failure<Unit>> Parse(string? source)
                    =>
                    default;
            }

            public readonly record struct FlatArray<T>(IReadOnlyList<T> Items);
        }
        """;

    internal const string PictureSetGetFactoryHintName
        =
        "PictureSetGetEndpoint.g.cs";

    internal const string PictureSetGetInvokeHintName
        =
        "PictureSetGetEndpoint.Invoke.g.cs";

    internal const string PictureSetGetMetadataHintName
        =
        "PictureSetGetEndpoint.Metadata.g.cs";

    internal const string PictureSetGetFactorySourceCode
        =
        """
        // Auto-generated code by PrimeFuncPack
        #nullable enable

        using GarageGroup.Infra.Endpoint;
        using Microsoft.Extensions.Logging;
        using System;
        using System.Text.Json;

        namespace Demo;

        [EndpointMetadata("GET", "/pictures/{entity}/{id}")]
        [EndpointOperationMetadata("PictureSetGet", "GET", "/pictures/{entity}/{id}")]
        public sealed partial class PictureSetGetEndpoint : IEndpoint
        {
            public static PictureSetGetEndpoint Resolve(IServiceProvider? serviceProvider, IPictureSetGetFunc endpointFunc)
                =>
                new(
                    endpointFunc: endpointFunc ?? throw new ArgumentNullException(nameof(endpointFunc)),
                    logger: serviceProvider?.GetEndpointLogger<PictureSetGetEndpoint>());

            private static readonly JsonSerializerOptions SerializerOptions = EndpointDeserializer.CreateDeafultOptions();

            private readonly IPictureSetGetFunc endpointFunc;

            private readonly ILogger? logger;

            private PictureSetGetEndpoint(IPictureSetGetFunc endpointFunc, ILogger? logger)
            {
                this.endpointFunc = endpointFunc;
                this.logger = logger;
            }
        }
        """;

    internal const string PictureSetGetInvokeSourceCode
        =
        """
        // Auto-generated code by PrimeFuncPack
        #nullable enable

        using GarageGroup.Infra;
        using GarageGroup.Infra.Endpoint;
        using Microsoft.Extensions.Logging;
        using System;
        using System.Collections.Generic;
        using System.Threading;
        using System.Threading.Tasks;

        namespace Demo;

        partial class PictureSetGetEndpoint
        {
            public async Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
            {
                var inputResult = MapRequest(request);
                if (inputResult.IsFailure)
                {
                    var inputFailure = inputResult.FailureOrThrow();

                    logger?.LogError(inputFailure.SourceException, "Request is incorrect: {failureMessage}", inputFailure.FailureMessage);
                    return inputResult.FailureOrThrow().ToBadRequestResponse(SerializerOptions);
                }

                var input = inputResult.SuccessOrThrow();
                var endpointResult = await endpointFunc.InvokeAsync(input, cancellationToken).ConfigureAwait(false);

                return MapSuccess(endpointResult);
            }

            private static Result<PictureSetGetIn, Failure<Unit>> MapRequest(EndpointRequest request)
            {
                var entityResult = EndpointParser.ParseEnum<PictureEntityType>(request.GetRouteValue("entity"));
                if (entityResult.IsFailure)
                {
                    return entityResult.FailureOrThrow();
                }

                var idResult = EndpointParser.ParseInt32(request.GetRouteValue("id"));
                if (idResult.IsFailure)
                {
                    return idResult.FailureOrThrow();
                }

                var typeResult = EndpointParser.ParseNullableEnum<PictureType>(request.GetQueryParameterValue("type"));
                if (typeResult.IsFailure)
                {
                    return typeResult.FailureOrThrow();
                }

                var formatsResult = JsonStringValue<FlatArray<String>>.Parse(request.GetQueryParameterValue("formats"));
                if (formatsResult.IsFailure)
                {
                    return formatsResult.FailureOrThrow();
                }

                return new PictureSetGetIn(
                    entity: entityResult.SuccessOrThrow(),
                    id: idResult.SuccessOrThrow(),
                    type: typeResult.SuccessOrThrow(),
                    formats: formatsResult.SuccessOrThrow());
            }

            private EndpointResponse MapSuccess(PictureSetGetOut success)
            {
                return new(
                    statusCode: 200,
                    headers: new KeyValuePair<string, string?>[]
                    {
                        new("Content-Type", "application/json")
                    },
                    body: success.Pictures.ToJsonStream(SerializerOptions));
            }
        }
        """;

    internal const string PictureSetGetMetadataSourceCode
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
        
        partial class PictureSetGetEndpoint
        {
            public static EndpointMetadata GetEndpointMetadata()
                =>
                new(
                    method: EndpointMethod.Get,
                    route: "/pictures/{entity}/{id}",
                    summary: default,
                    description: default,
                    operation: new()
                    {
                        OperationId = BuildOperationId("PictureSetGet"),
                        Summary = "Get pictures",
                        Description = "Returns pictures in required format",
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(
                                "Detailed information",
                                new()
                                {
                                    Tags = new HashSet<OpenApiTag>
                                    {
                                        new()
                                        {
                                            Name = "Detailed information",
                                            Description = "Methods that return detailed information about objects"
                                        }
                                    },
                                },
                                null)
                        },
                        Parameters =
                        [
                            new OpenApiParameter()
                            {
                                Required = true,
                                In = ParameterLocation.Path,
                                Name = "entity",
                                Schema = CreateEnumSchema<PictureEntityType>(false, example: default, description: default),
                                Description = "Entity type"
                            },
                            new OpenApiParameter()
                            {
                                Required = true,
                                In = ParameterLocation.Path,
                                Name = "id",
                                Schema = CreateInt32Schema(false, example: default, description: default),
                                Description = "Entity ID"
                            },
                            new OpenApiParameter()
                            {
                                Required = false,
                                In = ParameterLocation.Query,
                                Name = "type",
                                Schema = CreateEnumSchema<PictureType>(true, example: default, description: default),
                                Description = "Picture type"
                            },
                            new OpenApiParameter()
                            {
                                Required = false,
                                In = ParameterLocation.Query,
                                Name = "formats",
                                Schema = CreateDefaultSchema(true, example: CreateStringExample("[\"150x90_60\",\"634x468_60\"]"), description: default),
                                Description = "Picture format(s)"
                            }
                        ],
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
                                        ["items"] = new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.Array,
                                            Items = new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.Object,
                                                Properties = new Dictionary<string, IOpenApiSchema>
                                                {
                                                    ["url"] = CreateStringSchema(false, example: default, description: "Picture URL"),
                                                    ["type"] = CreateEnumSchema<PictureType>(true, example: default, description: "Picture type"),
                                                    ["format"] = CreateStringSchema(false, example: default, description: "Picture format"),
                                                    ["constructionDate"] = CreateDateSchema(true, example: default, description: "Picture construction date"),
                                                }
                                            },
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
                    OperationId = "PictureSetGet"
                };
        }
        """;
}
