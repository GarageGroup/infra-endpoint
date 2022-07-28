using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

[Generator]
internal sealed class ApplicationSwaggerSourceGenerator : ISourceGenerator
{
    private const string ApplicationSwaggerTypeName = "ApplicationSwagger";

    public void Execute(GeneratorExecutionContext context)
    {
        var source = context.BuildSource(ApplicationSwaggerTypeName);
        context.AddSource($"{ApplicationSwaggerTypeName}.g.cs", source);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }
}