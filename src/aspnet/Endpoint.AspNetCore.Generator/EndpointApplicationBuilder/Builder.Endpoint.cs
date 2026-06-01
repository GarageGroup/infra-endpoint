using PrimeFuncPack;

namespace GarageGroup.Infra;

partial class EndpointApplicationBuilder
{
    internal static string BuildEndpointSourceCode(this RootTypeMetadata rootType, ResolverMethodMetadata resolverMethod)
        =>
        new SourceBuilder(
            rootType.Namespace)
        .AddUsing(
            "Microsoft.AspNetCore.Builder")
        .AppendCodeLines(
            $"partial class {rootType.TypeName}")
        .BeginCodeBlock()
        .AppendCodeLines(
            $"internal static TBuilder {resolverMethod.MethodName}<TBuilder>(this TBuilder builder) where TBuilder : IApplicationBuilder")
        .BeginLambda()
        .AppendCodeLines(
            $"builder.{resolverMethod.GetUseMethodName()}({rootType.ProviderType.DisplayedTypeName}.{resolverMethod.MethodName}().Resolve);")
        .EndLambda()
        .EndCodeBlock()
        .Build();

    private static string GetUseMethodName(this ResolverMethodMetadata resolverMethod)
        =>
        resolverMethod.IsEndpointSet ? "UseEndpointSet" : "UseEndpoint";
}