using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class SourceGeneratorExtensions
{
    internal static IReadOnlyCollection<RootTypeMetadata> GetRootTypes(this GeneratorExecutionContext context)
    {
        var visitor = new ExportedTypesCollector(context.CancellationToken);
        visitor.VisitNamespace(context.Compilation.GlobalNamespace);

        return visitor.GetNonPrivateTypes().Select(GetRootType).NotNull().ToArray();
    }

    private static RootTypeMetadata? GetRootType(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.TypeArguments.Any())
        {
            return null;
        }

        var resolverMethodNames = typeSymbol.GetMembers().OfType<IMethodSymbol>().Select(GetResolverMethodName).NotEmpty().ToArray();
        if (resolverMethodNames.Any() is false)
        {
            return null;
        }

        return new(
            @namespace: typeSymbol.ContainingNamespace.ToString(),
            typeName: typeSymbol.Name + "EndpointExtensions",
            providerType: typeSymbol.GetDisplayedData(),
            resolverMethodNames: resolverMethodNames);
    }

    private static string? GetResolverMethodName(IMethodSymbol methodSymbol)
    {
        var extensionAttribute = methodSymbol.GetAttributes().FirstOrDefault(IsEndpointApplicationExtensionAttribute);
        if (extensionAttribute is null)
        {
            return null;
        }

        if (methodSymbol.IsStatic is false)
        {
            throw methodSymbol.CreateInvalidMethodException("must be static");
        }

        if (methodSymbol.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal))
        {
            throw methodSymbol.CreateInvalidMethodException("must be public or internal");
        }

        if (methodSymbol.TypeParameters.Any())
        {
            throw methodSymbol.CreateInvalidMethodException("must have no generic arguments");
        }

        if (methodSymbol.Parameters.Any())
        {
            throw methodSymbol.CreateInvalidMethodException("must have no parameters");
        }

        var returnType = methodSymbol.ReturnType as INamedTypeSymbol;
        if (returnType?.IsType("PrimeFuncPack", "Dependency") is not true || returnType?.TypeArguments.Length is not 1)
        {
            throw methodSymbol.CreateInvalidMethodException("return type must be PrimeFuncPack.Dependency<TEndpoint>");
        }

        var endpointType = returnType.TypeArguments[0] as INamedTypeSymbol;
        if (endpointType?.AllInterfaces.Any(IsEndpointType) is not true)
        {
            throw methodSymbol.CreateInvalidMethodException($"must resolve a type that implements {EndpointNamespace}.IEndpoint");
        }

        return methodSymbol.Name;

        static bool IsEndpointApplicationExtensionAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsType(DefaultNamespace, "EndpointApplicationExtensionAttribute") is true;

        static bool IsEndpointType(INamedTypeSymbol typeSymbol)
            =>
            typeSymbol.IsType(EndpointNamespace, "IEndpoint");
    }
}