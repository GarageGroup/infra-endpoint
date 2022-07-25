using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal static partial class SourceGeneratorExtensions
{
    private const string EndpointMetadataProviderInterfaceName = "GGroupp.Infra.Endpoint.IEndpointMetadataProvider";

    internal static IReadOnlyCollection<INamedTypeSymbol> GetEndpointTypes(this GeneratorExecutionContext context)
    {
        var metadataProviderType = context.Compilation.GetTypeByMetadataNameOrThrow(EndpointMetadataProviderInterfaceName);

        var visitor = new ExportedTypesCollector(context.CancellationToken);

        visitor.VisitAssembly(context.Compilation.Assembly);
        foreach (var assembly in context.Compilation.SourceModule.ReferencedAssemblySymbols)
        {
            visitor.VisitAssembly(assembly);
        }

        return visitor.GetPublicTypes().Where(IsProviderImplicit).ToArray();

        bool IsProviderImplicit(INamedTypeSymbol classSymbol)
            =>
            context.IsImplict(classSymbol, metadataProviderType);
    }

    private static bool IsImplict(this GeneratorExecutionContext context, INamedTypeSymbol classSymbol, ITypeSymbol baseType)
    {
        var conversation = context.Compilation.ClassifyCommonConversion(classSymbol, baseType);
        return conversation.Exists && conversation.IsImplicit;
    }

    private static INamedTypeSymbol GetTypeByMetadataNameOrThrow(this Compilation compilation, string fullyQualifiedMetadataName)
        =>
        compilation.GetTypeByMetadataName(fullyQualifiedMetadataName)
            ?? throw new InvalidOperationException($"Interface '{fullyQualifiedMetadataName}' was not found in compilation");
}