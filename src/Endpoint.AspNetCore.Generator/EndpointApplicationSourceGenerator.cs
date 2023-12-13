using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

[Generator]
internal sealed class EndpointApplicationSourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var rootType in context.GetRootTypes())
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

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }
}