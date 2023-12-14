namespace GarageGroup.Infra;

partial class EndpointApplicationBuilder
{
    internal static string BuildEndpointSourceCode(this RootTypeMetadata rootType, string resolverMethodName)
        =>
        new SourceBuilder(
            rootType.Namespace)
        .AddUsing(
            "Microsoft.AspNetCore.Builder")
        .AppendCodeLine(
            $"partial class {rootType.TypeName}")
        .BeginCodeBlock()
        .AppendCodeLine(
            $"internal static TBuilder {resolverMethodName}<TBuilder>(this TBuilder builder) where TBuilder : IApplicationBuilder")
        .BeginLambda()
        .AppendCodeLine(
            $"builder.UseEndpoint({rootType.ProviderType.DisplayedTypeName}.{resolverMethodName}().Resolve);")
        .EndLambda()
        .EndCodeBlock()
        .Build();
}