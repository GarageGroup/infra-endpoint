using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using PrimeFuncPack;

namespace GarageGroup.Infra;

partial class SourceGeneratorExtensions
{
    internal static IReadOnlyCollection<RootTypeMetadata> GetRootTypes(this Compilation compilation, CancellationToken cancellationToken)
    {
        var visitor = new ExportedTypesCollector(cancellationToken);
        visitor.VisitNamespace(compilation.GlobalNamespace);

        return visitor.GetNonPrivateTypes().Select(GetRootType).NotNull().ToArray();
    }

    private static RootTypeMetadata? GetRootType(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.TypeArguments.Any())
        {
            return null;
        }

        var resolverMethods = typeSymbol.GetMembers().OfType<IMethodSymbol>().Select(GetResolverMethod).NotNull().ToArray();
        if (resolverMethods.Any() is false)
        {
            return null;
        }

        return new(
            @namespace: typeSymbol.ContainingNamespace.ToString(),
            typeName: typeSymbol.Name + "EndpointExtensions",
            providerType: typeSymbol.GetDisplayedData(),
            resolverMethods: resolverMethods);
    }

    private static ResolverMethodMetadata? GetResolverMethod(IMethodSymbol methodSymbol)
    {
        var attributes = methodSymbol.GetAttributes().ToArray();
        var hasEndpointAttribute = attributes.Any(IsEndpointApplicationExtensionAttribute);
        var hasEndpointSetAttribute = attributes.Any(IsEndpointSetApplicationExtensionAttribute);

        if (hasEndpointAttribute is false && hasEndpointSetAttribute is false)
        {
            return null;
        }

        if (hasEndpointAttribute && hasEndpointSetAttribute)
        {
            throw methodSymbol.CreateInvalidMethodException(
                $"must have only one of {DefaultNamespace}.EndpointApplicationExtensionAttribute or {DefaultNamespace}.EndpointSetApplicationExtensionAttribute");
        }

        var isEndpointSet = hasEndpointSetAttribute;

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

        var endpointType = GetResolvedHandlerType(methodSymbol.ReturnType, methodSymbol);
        if (endpointType.AllInterfaces.Any(IsEndpointType) is not true)
        {
            var interfaceName = isEndpointSet ? "IEndpointSet" : "IEndpoint";
            throw methodSymbol.CreateInvalidMethodException($"must resolve a type that implements {EndpointNamespace}.{interfaceName}");
        }

        return new(
            methodName: methodSymbol.Name,
            isEndpointSet: isEndpointSet);

        static bool IsEndpointApplicationExtensionAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsType(DefaultNamespace, "EndpointApplicationExtensionAttribute") is true;

        static bool IsEndpointSetApplicationExtensionAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsType(DefaultNamespace, "EndpointSetApplicationExtensionAttribute") is true;

        bool IsEndpointType(INamedTypeSymbol typeSymbol)
            =>
            isEndpointSet
                ? typeSymbol.IsType(EndpointNamespace, "IEndpointSet")
                : typeSymbol.IsType(EndpointNamespace, "IEndpoint");
    }

    private static INamedTypeSymbol GetResolvedHandlerType(ITypeSymbol dependencyType, IMethodSymbol methodSymbol)
    {
        if (dependencyType is not INamedTypeSymbol namedDependencyType)
        {
            throw methodSymbol.CreateInvalidMethodException(
                "return type must be a named type with public instance Resolve(System.IServiceProvider) method");
        }

        var resolveMethod = EnumerateResolveMethods(namedDependencyType).FirstOrDefault(IsResolveContractMatch)
            ?? throw methodSymbol.CreateInvalidMethodException(
                "return type must contain a public instance Resolve(System.IServiceProvider) method without generic arguments");

        if (resolveMethod.ReturnType is not INamedTypeSymbol handlerType)
        {
            throw methodSymbol.CreateInvalidMethodException(
                "Resolve(System.IServiceProvider) must return a named type");
        }

        return handlerType;

        static IEnumerable<IMethodSymbol> EnumerateResolveMethods(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind is TypeKind.Interface)
            {
                foreach (var methodSymbol in typeSymbol.GetMembers("Resolve").OfType<IMethodSymbol>())
                {
                    yield return methodSymbol;
                }

                foreach (var interfaceSymbol in typeSymbol.AllInterfaces)
                {
                    foreach (var methodSymbol in interfaceSymbol.GetMembers("Resolve").OfType<IMethodSymbol>())
                    {
                        yield return methodSymbol;
                    }
                }

                yield break;
            }

            for (var currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
            {
                foreach (var methodSymbol in currentType.GetMembers("Resolve").OfType<IMethodSymbol>())
                {
                    yield return methodSymbol;
                }
            }
        }

        static bool IsResolveContractMatch(IMethodSymbol methodSymbol)
        {
            if (methodSymbol.MethodKind is not MethodKind.Ordinary)
            {
                return false;
            }

            if (methodSymbol.IsStatic)
            {
                return false;
            }

            if (methodSymbol.DeclaredAccessibility is not Accessibility.Public)
            {
                return false;
            }

            if (methodSymbol.TypeParameters.Any())
            {
                return false;
            }

            if (methodSymbol.Parameters.Length is not 1)
            {
                return false;
            }

            var parameterSymbol = methodSymbol.Parameters[0];
            if (parameterSymbol.RefKind is not RefKind.None)
            {
                return false;
            }

            if (parameterSymbol.Type.IsType("System", "IServiceProvider") is false)
            {
                return false;
            }

            return methodSymbol.ReturnsVoid is false;
        }
    }

    private static IEnumerable<ResolverMethodMetadata> NotNull(this IEnumerable<ResolverMethodMetadata?> source)
    {
        foreach (var item in source)
        {
            if (item is null)
            {
                continue;
            }

            yield return item;
        }
    }
}