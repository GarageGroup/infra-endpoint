using PrimeFuncPack;

namespace GarageGroup.Infra;

partial class EndpointApplicationBuilder
{
    internal static string BuildConstructorSourceCode(this RootTypeMetadata rootType)
        =>
        new SourceBuilder(
            rootType.Namespace)
        .AppendCodeLines(
            $"internal static partial class {rootType.TypeName}")
        .BeginCodeBlock()
        .EndCodeBlock()
        .Build();
}