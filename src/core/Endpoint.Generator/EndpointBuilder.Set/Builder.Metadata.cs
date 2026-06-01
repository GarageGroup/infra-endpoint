using System.Linq;
using PrimeFuncPack;

namespace GarageGroup.Infra;

partial class EndpointSetBuilder
{
    internal static string BuildEndpointSetMetadataSource(this EndpointSetTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System.Collections.Generic",
            "GarageGroup.Infra.Endpoint")
        .AppendCodeLines(
            $"partial class {type.TypeEndpointSetName}")
        .BeginCodeBlock()
        .AppendCodeLines(
            "public static IReadOnlyCollection<EndpointMetadata> Metadata { get; }")
        .BeginArguments()
        .AppendCodeLines(
            "=")
        .BeginCollectionExpression()
        .AppendOperationsMetadataSourceCode(
            type.Endpoints?.ToArray() ?? [])
        .EndCollectionExpression(";")
        .EndArguments()
        .EndCodeBlock()
        .Build();

    private static SourceBuilder AppendOperationsMetadataSourceCode(
        this SourceBuilder builder, EndpointSetEndpointDescription[] endpoints)
    {
        for (var i = 0; i < endpoints.Length; i++)
        {
            var endpoint = endpoints[i];
            var lineEnd = i < endpoints.Length - 1 ? "," : string.Empty;

            builder = builder
                .AddUsing(
                    endpoint.EndpointNamespace ?? string.Empty)
                .AppendCodeLines(
                    $"{endpoint.EndpointTypeName}.GetEndpointMetadata(){lineEnd}");
        }

        return builder;
    }
}