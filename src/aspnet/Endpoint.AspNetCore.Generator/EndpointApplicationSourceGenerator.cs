using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

[Generator(LanguageNames.CSharp)]
internal sealed class EndpointApplicationSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var rootTypes = context.CompilationProvider.SelectMany(SourceGeneratorExtensions.GetRootTypes);
        context.RegisterSourceOutput(rootTypes, AddSources);
    }

    private static void AddSources(SourceProductionContext context, RootTypeMetadata rootType)
    {
        var constructorSourceCode = rootType.BuildConstructorSourceCode();
        context.AddSource($"{rootType.TypeName}.g.cs", constructorSourceCode);

        foreach (var resolverMethodName in rootType.ResolverMethodNames)
        {
            var endpointSourceCode = rootType.BuildEndpointSourceCode(resolverMethodName);
            context.AddSource($"{rootType.TypeName}.{resolverMethodName}.g.cs", endpointSourceCode);
        }
    }
}