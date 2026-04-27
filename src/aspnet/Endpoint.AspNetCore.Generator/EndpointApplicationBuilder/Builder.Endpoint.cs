using PrimeFuncPack;

namespace GarageGroup.Infra;

partial class EndpointApplicationBuilder
{
    internal static string BuildEndpointSourceCode(this RootTypeMetadata rootType, string resolverMethodName)
        =>
        new SourceBuilder(
            rootType.Namespace)
        .AddUsing(
            "Microsoft.AspNetCore.Builder")
        .AppendCodeLines(
            $"partial class {rootType.TypeName}")
        .BeginCodeBlock()
        .AppendCodeLines(
            $"internal static TBuilder {resolverMethodName}<TBuilder>(this TBuilder builder) where TBuilder : IApplicationBuilder")
        .BeginLambda()
        .AppendCodeLines(
            $"builder.UseEndpoint({rootType.ProviderType.DisplayedTypeName}.{resolverMethodName}().Resolve);")
        .EndLambda()
        .EndCodeBlock()
        .Build();
}