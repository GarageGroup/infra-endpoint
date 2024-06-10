namespace GarageGroup.Infra;

partial class EndpointBuilder
{
    internal static string BuildSource(this MediaTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System.Collections.Generic",
            "Microsoft.OpenApi.Models")
        .AddAlias(
            "static GarageGroup.Infra.Endpoint.EndpointMetadataHelper")
        .AppendCodeLine(
            $"internal static class {type.TypeName}")
        .BeginCodeBlock()
        .AppendStaticConstructor(
            type)
        .AppendEmptyLine()
        .AppendCodeLine(
            $"public static OpenApiMediaType {MediaTypePropertyName} {{ get; }}")
        .EndCodeBlock()
        .Build();

    private static SourceBuilder AppendStaticConstructor(this SourceBuilder builder, MediaTypeDescription type)
        =>
        builder.AppendCodeLine(
            $"static {type.TypeName}()")
        .BeginLambda()
        .AppendCodeLine(
            $"{MediaTypePropertyName} = new()")
        .BeginCodeBlock()
        .AppendSchema(
            "Schema", type.Type, 0, default, default, default)
        .EndCodeBlock(';')
        .EndLambda();
}