using PrimeFuncPack;

namespace GarageGroup.Infra;

partial class EndpointBuilder
{
    internal static string BuildSource(this MediaTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System.Collections.Generic",
            "Microsoft.OpenApi")
        .AddAlias(
            "static GarageGroup.Infra.Endpoint.EndpointMetadataHelper")
        .AppendCodeLines(
            $"internal static class {type.TypeName}")
        .BeginCodeBlock()
        .AppendStaticConstructor(
            type)
        .AppendEmptyLine()
        .AppendCodeLines(
            $"public static OpenApiMediaType {MediaTypePropertyName} {{ get; }}")
        .EndCodeBlock()
        .Build();

    private static SourceBuilder AppendStaticConstructor(this SourceBuilder builder, MediaTypeDescription type)
        =>
        builder.AppendCodeLines(
            $"static {type.TypeName}()")
        .BeginLambda()
        .AppendCodeLines(
            $"{MediaTypePropertyName} = new()")
        .BeginCodeBlock()
        .AppendSchema(
            "Schema", type.Type, 0, default, default, default)
        .EndCodeBlock(";")
        .EndLambda();
}