namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string NotificationSubscribeSourceCode
        =
        """
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo
        {
            using static NotificationSubscribeMetadata;

            [Endpoint(EndpointMethod.Post, Func.RouteSubscribe, Summary = Func.SummarySubscribe, Description = Func.DescriptionSubscribe)]
            [EndpointTag(Func.Tag)]
            [EndpointTag("Audit", Description = "Audit events")]
            public interface INotificationSubscribeFunc
            {
                ValueTask<Result<Unit, Failure<NotificationSubscribeFailureCode>>> InvokeAsync(
                    NotificationSubscribeIn input, CancellationToken cancellationToken);
            }

            public sealed record class NotificationSubscribeIn
            {
                public NotificationSubscribeIn(
                    [ClaimIn] Guid systemUserId,
                    [RootBodyIn] BaseSubscriptionData subscriptionData)
                {
                    SystemUserId = systemUserId;
                    SubscriptionData = subscriptionData;
                }

                public Guid SystemUserId { get; }

                public BaseSubscriptionData SubscriptionData { get; }
            }

            public enum NotificationSubscribeFailureCode
            {
                Unknown,

                [Problem(FailureStatusCode.BadRequest, true)]
                InvalidQuery,

                [Problem(FailureStatusCode.BadRequest, FailureCode.NotificationTypeInvalidMessage)]
                NotificationTypeInvalid,

                [Problem(FailureStatusCode.NotFound, FailureCode.NotificationTypeNotFoundMessage)]
                NotificationTypeNotFound,

                [Problem(FailureStatusCode.NotFound, FailureCode.BotUserNotFoundMessage)]
                BotUserNotFound
            }

            public abstract record class BaseSubscriptionData : IEndpointBodyParser<BaseSubscriptionData>, IEndpointBodyMetadataProvider
            {
                public static object GetEndpointBodyMetadata()
                    =>
                    new();

                public static ValueTask<Result<BaseSubscriptionData, Failure<Unit>>> ParseAsync(
                    EndpointRequest request, CancellationToken cancellationToken)
                    =>
                    default;
            }

            internal static class NotificationSubscribeMetadata
            {
                internal static class Func
                {
                    public const string Tag = "Notification";

                    public const string RouteSubscribe = "/subscribeToNotification";

                    public const string SummarySubscribe = "Subscribe bot user to notification";

                    public const string DescriptionSubscribe = "Allows a bot user to subscribe to specific notifications";
                }

                internal static class FailureCode
                {
                    public const string NotificationTypeInvalidMessage = "Notification type is unknown";

                    public const string BotUserNotFoundMessage = "Bot user was not found";

                    public const string NotificationTypeNotFoundMessage = "Notification type was not found";
                }
            }
        }

        namespace GarageGroup.Infra
        {
            public sealed class EndpointRequest;

            public interface IEndpointBodyParser<TSelf>
                where TSelf : IEndpointBodyParser<TSelf>
            {
                static abstract ValueTask<Result<TSelf, Failure<Unit>>> ParseAsync(
                    EndpointRequest request, CancellationToken cancellationToken);
            }

            public interface IEndpointBodyMetadataProvider
            {
                static abstract object GetEndpointBodyMetadata();
            }
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

    internal const string NotificationSubscribeFactoryHintName
        =
        "NotificationSubscribeEndpoint.g.cs";

    internal const string NotificationSubscribeInvokeHintName
        =
        "NotificationSubscribeEndpoint.Invoke.g.cs";

    internal const string NotificationSubscribeMetadataHintName
        =
        "NotificationSubscribeEndpoint.Metadata.g.cs";

    internal const string NotificationSubscribeFactorySourceCode
        =
        """
        // Auto-generated code by PrimeFuncPack
        #nullable enable

        using GarageGroup.Infra.Endpoint;
        using Microsoft.Extensions.Logging;
        using System;
        using System.Text.Json;

        namespace Demo;

        [EndpointMetadata("POST", "/subscribeToNotification")]
        public sealed partial class NotificationSubscribeEndpoint : IEndpoint
        {
            public static NotificationSubscribeEndpoint Resolve(IServiceProvider? serviceProvider, INotificationSubscribeFunc endpointFunc)
                =>
                new(
                    endpointFunc: endpointFunc ?? throw new ArgumentNullException(nameof(endpointFunc)),
                    jsonSerializerOptions: DefaultSerializerOptions,
                    logger: serviceProvider?.GetEndpointLogger<NotificationSubscribeEndpoint>());

            private static readonly JsonSerializerOptions DefaultSerializerOptions = EndpointDeserializer.CreateDeafultOptions();

            private readonly INotificationSubscribeFunc endpointFunc;

            private readonly JsonSerializerOptions jsonSerializerOptions;

            private readonly ILogger? logger;

            private NotificationSubscribeEndpoint(INotificationSubscribeFunc endpointFunc, JsonSerializerOptions jsonSerializerOptions, ILogger? logger)
            {
                this.endpointFunc = endpointFunc;
                this.logger = logger;
                this.jsonSerializerOptions = jsonSerializerOptions;
            }
        }
        """;

    internal const string NotificationSubscribeInvokeSourceCode
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

        partial class NotificationSubscribeEndpoint
        {
            public async Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
            {
                var inputResult = await MapRequestAsync(request, cancellationToken).ConfigureAwait(false);
                if (inputResult.IsFailure)
                {
                    var inputFailure = inputResult.FailureOrThrow();

                    logger?.LogError(inputFailure.SourceException, "Request is incorrect: {failureMessage}", inputFailure.FailureMessage);
                    return inputResult.FailureOrThrow().ToBadRequestResponse(jsonSerializerOptions);
                }

                var input = inputResult.SuccessOrThrow();
                var endpointResult = await endpointFunc.InvokeAsync(input, cancellationToken).ConfigureAwait(false);

                return endpointResult.Fold(MapSuccess, MapFailure);
            }

            private async ValueTask<Result<NotificationSubscribeIn, Failure<Unit>>> MapRequestAsync(EndpointRequest request, CancellationToken token)
            {
                var systemUserIdResult = EndpointParser.ParseGuid(request.GetClaimValue("systemUserId"));
                if (systemUserIdResult.IsFailure)
                {
                    return systemUserIdResult.FailureOrThrow();
                }

                var subscriptionDataResult = await BaseSubscriptionData.ParseAsync(request, token).ConfigureAwait(false);
                if (subscriptionDataResult.IsFailure)
                {
                    return subscriptionDataResult.FailureOrThrow();
                }

                return new NotificationSubscribeIn(
                    systemUserId: systemUserIdResult.SuccessOrThrow(),
                    subscriptionData: subscriptionDataResult.SuccessOrThrow());
            }

            private EndpointResponse MapSuccess(Unit success)
            {
                return new(
                    statusCode: 204,
                    headers: default,
                    body: default);
            }

            private EndpointResponse MapFailure(Failure<NotificationSubscribeFailureCode> failure)
            {
                if (failure.FailureCode is NotificationSubscribeFailureCode.InvalidQuery)
                {
                    LogUnexpectedStatusCode(400, failure.SourceException, failure.FailureMessage);

                    return new EndpointProblem(
                        type: "BadRequest",
                        title: "about:blank",
                        status: 400,
                        detail: failure.FailureMessage)
                    .ToFailureResponse(jsonSerializerOptions);
                }

                if (failure.FailureCode is NotificationSubscribeFailureCode.NotificationTypeInvalid)
                {
                    LogUnexpectedStatusCode(400, failure.SourceException, failure.FailureMessage);

                    return new EndpointProblem(
                        type: "BadRequest",
                        title: "about:blank",
                        status: 400,
                        detail: "Notification type is unknown")
                    .ToFailureResponse(jsonSerializerOptions);
                }

                if (failure.FailureCode is NotificationSubscribeFailureCode.NotificationTypeNotFound)
                {
                    LogUnexpectedStatusCode(404, failure.SourceException, failure.FailureMessage);

                    return new EndpointProblem(
                        type: "NotFound",
                        title: "about:blank",
                        status: 404,
                        detail: "Notification type was not found")
                    .ToFailureResponse(jsonSerializerOptions);
                }

                if (failure.FailureCode is NotificationSubscribeFailureCode.BotUserNotFound)
                {
                    LogUnexpectedStatusCode(404, failure.SourceException, failure.FailureMessage);

                    return new EndpointProblem(
                        type: "NotFound",
                        title: "about:blank",
                        status: 404,
                        detail: "Bot user was not found")
                    .ToFailureResponse(jsonSerializerOptions);
                }

                logger?.LogError(failure.SourceException, "An unexpected http error: {errorCode}. Message: {message}", failure.FailureCode, failure.FailureMessage);
                return new(500, default, default);

                void LogUnexpectedStatusCode(int code, Exception? sourceException, string failureMessage)
                    =>
                    logger?.LogInformation(sourceException, "An unsuccessful status code: {statusCode}. Message: {message}", code, failureMessage);
            }
        }
        """;

    internal const string NotificationSubscribeMetadataSourceCode
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
        
        partial class NotificationSubscribeEndpoint
        {
            public static EndpointMetadata GetEndpointMetadata()
                =>
                new(
                    method: EndpointMethod.Post,
                    route: "/subscribeToNotification",
                    summary: default,
                    description: default,
                    operation: new()
                    {
                        Summary = "Subscribe bot user to notification",
                        Description = "Allows a bot user to subscribe to specific notifications",
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(
                                "Notification",
                                new()
                                {
                                    Tags = new HashSet<OpenApiTag>
                                    {
                                        new()
                                        {
                                            Name = "Notification",
                                            Description = default
                                        }
                                    },
                                },
                                null),
                            new(
                                "Audit",
                                new()
                                {
                                    Tags = new HashSet<OpenApiTag>
                                    {
                                        new()
                                        {
                                            Name = "Audit",
                                            Description = "Audit events"
                                        }
                                    },
                                },
                                null)
                        },
                        RequestBody = BaseSubscriptionData.GetEndpointBodyMetadata(),
                        Responses = new()
                        {
                            ["204"] = new OpenApiResponse()
                            {
                                Description = "NoContent",
                            },
                            ["400"] = new OpenApiResponse()
                            {
                                Description = "BadRequest",
                                Content = CreateProblemContent()
                            },
                            ["404"] = new OpenApiResponse()
                            {
                                Description = "NotFound",
                                Content = CreateProblemContent()
                            }
                        }
                    },
                    schemas: new Dictionary<string, IOpenApiSchema>()
                    {
                        ["ProblemDetails"] = CreateProblemSchema()
                    });
        }
        """;
}