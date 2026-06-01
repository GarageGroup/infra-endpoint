using System.Collections.Generic;
using PrimeFuncPack;

namespace GarageGroup.Infra;

partial class EndpointSetBuilder
{
    internal static string BuildEndpointSetFactorySource(this EndpointSetTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System",
            "GarageGroup.Infra.Endpoint",
            "Microsoft.Extensions.Logging")
        .AppendEndpointOperationMetadataAttributes(
            type.Endpoints)
        .AppendCodeLines(
            $"{type.GetVisibility()} sealed partial class {type.TypeEndpointSetName} : IEndpointSet")
        .BeginCodeBlock()
        .AppendCodeLines(
            $"{type.GetVisibility()} static {type.TypeEndpointSetName} Resolve(IServiceProvider? serviceProvider, {type.TypeFuncName} endpointApi)")
        .BeginLambda()
        .AppendCodeLines(
            "new(")
        .BeginArguments()
        .AppendCodeLines(
            $"endpointApi: {GetNullValidationValue("endpointApi", type.IsTypeFuncStruct)},",
            $"logger: serviceProvider?.GetEndpointLogger<{type.TypeEndpointSetName}>());")
        .EndArguments()
        .EndLambda()
        .AppendEmptyLine()
        .AppendCodeLines(
            $"private readonly {type.TypeFuncName} endpointApi;")
        .AppendEmptyLine()
        .AppendCodeLines(
            "private readonly ILogger? logger;")
        .AppendEmptyLine()
        .AppendCodeLines(
            $"private {type.TypeEndpointSetName}({type.TypeFuncName} endpointApi, ILogger? logger)")
        .BeginCodeBlock()
        .AppendCodeLines(
            "this.endpointApi = endpointApi;",
            "this.logger = logger;")
        .EndCodeBlock()
        .EndCodeBlock()
        .Build();

    private static SourceBuilder AppendEndpointOperationMetadataAttributes(
        this SourceBuilder builder,
        IReadOnlyCollection<EndpointSetEndpointDescription>? endpoints)
    {
        if (endpoints is null)
        {
            return builder;
        }

        foreach (var endpoint in endpoints)
        {
            var method = endpoint.MethodName?.ToUpperInvariant();
            builder = builder.AppendCodeLines(
                $"[EndpointOperationMetadata({endpoint.OperationId.AsStringSourceCodeOr()}, {method.AsStringSourceCodeOr()}, {endpoint.Route.AsStringSourceCodeOr()})]");
        }

        return builder;
    }
}
