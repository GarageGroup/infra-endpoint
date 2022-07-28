using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal static partial class SourceGeneratorExtensions
{
    internal static string GetNamespace(this GeneratorExecutionContext context)
    {
        var entryPointNamespace = context.Compilation.GetEntryPoint(context.CancellationToken)?.ContainingNamespace.ToDisplayString();

        return string.IsNullOrEmpty(entryPointNamespace) ? "GGroupp.Infra" : entryPointNamespace!;
    }
}