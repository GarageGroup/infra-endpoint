using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal static partial class SourceGeneratorExtensions
{
    private const string EndpointMetadataProviderInterfaceName = "GGroupp.Infra.Endpoint.IEndpointMetadataProvider";

    private const string SwaggerConfiguratorInterfaceName = "GGroupp.Infra.ISwaggerConfigurator";

    private static IReadOnlyCollection<INamedTypeSymbol> GetParentTypes(this GeneratorExecutionContext context, INamedTypeSymbol baseType)
    {
        var visitor = new ExportedTypesCollector(context.CancellationToken);

        visitor.VisitAssembly(context.Compilation.Assembly);
        foreach (var assembly in context.Compilation.SourceModule.ReferencedAssemblySymbols)
        {
            visitor.VisitAssembly(assembly);
        }

        return visitor.GetPublicTypes().Where(IsParentType).ToArray();

        bool IsParentType(INamedTypeSymbol classSymbol)
            =>
            context.IsImplict(classSymbol, baseType);
    }

    private static bool IsImplict(this GeneratorExecutionContext context, INamedTypeSymbol classSymbol, ITypeSymbol baseType)
    {
        var conversation = context.Compilation.ClassifyCommonConversion(classSymbol, baseType);
        return conversation.Exists && conversation.IsImplicit;
    }
}