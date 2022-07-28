using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal static partial class SourceGeneratorExtensions
{
    internal static IReadOnlyCollection<INamedTypeSymbol> GetConfigurators(this GeneratorExecutionContext context)
    {
        var swaggerConfiguratorType = context.Compilation.GetTypeByMetadataName(SwaggerConfiguratorInterfaceName);
        if (swaggerConfiguratorType is null)
        {
            return Array.Empty<INamedTypeSymbol>();
        }

        return context.GetParentTypes(swaggerConfiguratorType);
    }
}