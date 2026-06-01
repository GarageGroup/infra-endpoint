namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string CustomSerializerOptionsSourceCode
        =
        """
        using System.Text.Json;
        using System.Threading;
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo
        {
            [Endpoint("CustomSerializerOptionsGet", EndpointMethod.Post, "/custom/{id}")]
            public sealed class CustomSerializerOptionsGetFunc
            {
                internal static JsonSerializerOptions? CustomSerializerOptions
                    =>
                    default;

                public ValueTask<CustomSerializerOptionsGetOut> InvokeAsync(
                    CustomSerializerOptionsGetIn input, CancellationToken cancellationToken)
                    =>
                    default;
            }

            public sealed record class CustomSerializerOptionsGetIn([RouteIn] int Id);

            public sealed record class CustomSerializerOptionsGetOut
            {
                [RootBodyOut]
                public CustomSerializerOptionsGetBody Body { get; init; }
            }

            public sealed record class CustomSerializerOptionsGetBody
            {
                public string Value { get; init; }
            }
        }
        """;

    internal const string CustomSerializerOptionsFactoryHintName
        =
        "CustomSerializerOptionsGetEndpoint.g.cs";

    internal const string CustomSerializerOptionsInvokeHintName
        =
        "CustomSerializerOptionsGetEndpoint.Invoke.g.cs";

    internal const string CustomSerializerOptionsFactorySourceCode
        =
        """
        // Auto-generated code by PrimeFuncPack
        #nullable enable

        using GarageGroup.Infra.Endpoint;
        using Microsoft.Extensions.Logging;
        using System;
        using System.Text.Json;

        namespace Demo;

        [EndpointMetadata("POST", "/custom/{id}")]
        [EndpointOperationMetadata("CustomSerializerOptionsGet", "POST", "/custom/{id}")]
        public sealed partial class CustomSerializerOptionsGetEndpoint : IEndpoint
        {
            public static CustomSerializerOptionsGetEndpoint Resolve(IServiceProvider? serviceProvider, CustomSerializerOptionsGetFunc endpointFunc)
                =>
                new(
                    endpointFunc: endpointFunc ?? throw new ArgumentNullException(nameof(endpointFunc)),
                    logger: serviceProvider?.GetEndpointLogger<CustomSerializerOptionsGetEndpoint>());

            private static readonly JsonSerializerOptions SerializerOptions = CustomSerializerOptionsGetFunc.CustomSerializerOptions ?? EndpointDeserializer.CreateDeafultOptions();

            private readonly CustomSerializerOptionsGetFunc endpointFunc;

            private readonly ILogger? logger;

            private CustomSerializerOptionsGetEndpoint(CustomSerializerOptionsGetFunc endpointFunc, ILogger? logger)
            {
                this.endpointFunc = endpointFunc;
                this.logger = logger;
            }
        }
        """;

    internal const string CustomSerializerOptionsInvokeSourceCode
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

        partial class CustomSerializerOptionsGetEndpoint
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

            private static Result<CustomSerializerOptionsGetIn, Failure<Unit>> MapRequest(EndpointRequest request)
            {
                var IdResult = EndpointParser.ParseInt32(request.GetRouteValue("Id"));
                if (IdResult.IsFailure)
                {
                    return IdResult.FailureOrThrow();
                }

                return new CustomSerializerOptionsGetIn(
                    Id: IdResult.SuccessOrThrow());
            }

            private EndpointResponse MapSuccess(CustomSerializerOptionsGetOut success)
            {
                return new(
                    statusCode: 200,
                    headers: new KeyValuePair<string, string?>[]
                    {
                        new("Content-Type", "application/json")
                    },
                    body: success.Body.ToJsonStream(SerializerOptions));
            }
        }
        """;
}
