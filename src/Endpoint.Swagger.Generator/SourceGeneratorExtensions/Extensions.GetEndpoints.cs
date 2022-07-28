using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal static partial class SourceGeneratorExtensions
{
    internal static IReadOnlyCollection<INamedTypeSymbol> GetEndpointTypes(this GeneratorExecutionContext context)
    {
        var metadataProviderType = context.Compilation.GetTypeByMetadataNameOrThrow(EndpointMetadataProviderInterfaceName);
        return context.GetParentTypes(metadataProviderType);
    }

    private static INamedTypeSymbol GetTypeByMetadataNameOrThrow(this Compilation compilation, string fullyQualifiedMetadataName)
        =>
        compilation.GetTypeByMetadataName(fullyQualifiedMetadataName)
            ?? throw new InvalidOperationException($"Interface '{fullyQualifiedMetadataName}' was not found in compilation");
}