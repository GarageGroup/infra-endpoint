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

        foreach (var resolverMethod in rootType.ResolverMethods)
        {
            var endpointSourceCode = rootType.BuildEndpointSourceCode(resolverMethod);
            context.AddSource($"{rootType.TypeName}.{resolverMethod.MethodName}.g.cs", endpointSourceCode);
        }
    }
}