using System.Linq;
using PrimeFuncPack;

namespace GarageGroup.Infra;

partial class EndpointSetBuilder
{
    internal static string BuildEndpointSetInvokeSource(this EndpointSetTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System",
            "System.Threading",
            "System.Threading.Tasks",
            "GarageGroup.Infra.Endpoint",
            "Microsoft.Extensions.Logging")
        .AppendCodeLines(
            $"partial class {type.TypeEndpointSetName}")
        .BeginCodeBlock()
        .AppendCodeLines(
            "public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)")
        .BeginCodeBlock()
        .AppendInvokeSourceCode(
            type.Endpoints?.ToArray() ?? [])
        .AppendCodeLines(
            "logger?.LogError(\"An endpoint operation id is invalid: {operationId}\", request.OperationId);")
        .AppendCodeLines(
            "return Task.FromResult(new EndpointResponse(500, default, default));")
        .EndCodeBlock()
        .EndCodeBlock()
        .Build();

    private static SourceBuilder AppendInvokeSourceCode(
        this SourceBuilder builder, EndpointSetEndpointDescription[] endpoints)
    {
        foreach (var endpoint in endpoints)
        {
            builder = builder
                .AppendCodeLines(
                    $"if (string.Equals(request.OperationId, {endpoint.OperationId.AsStringSourceCodeOrStringEmpty()}, StringComparison.Ordinal))")
                .BeginCodeBlock()
                .AddUsing(
                    endpoint.EndpointNamespace ?? string.Empty)
                .AppendCodeLines(
                    $"return new {endpoint.EndpointTypeName}(endpointApi, logger).InvokeAsync(request, cancellationToken);")
                .EndCodeBlock()
                .AppendEmptyLine();
        }

        return builder;
    }
}