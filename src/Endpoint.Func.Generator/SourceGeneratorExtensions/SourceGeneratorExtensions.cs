using System;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal static partial class SourceGeneratorExtensions
{
    private const string EndpointPrefixName = "Gen";

    private static string GetTypeRootName(this string endpointTypeName)
    {
        if (string.IsNullOrEmpty(endpointTypeName))
        {
            throw new InvalidOperationException("Endpoint type name must be specified");
        }

        var httpFuncIndex = endpointTypeName.IndexOf("HttpFunc", StringComparison.InvariantCultureIgnoreCase);
        if (httpFuncIndex > 0)
        {
            return EndpointPrefixName + endpointTypeName.Substring(0, httpFuncIndex);
        }

        var funcIndex = endpointTypeName.IndexOf("Func", StringComparison.InvariantCultureIgnoreCase);
        if (funcIndex > 0)
        {
            return EndpointPrefixName + endpointTypeName.Substring(0, funcIndex);
        }

        return EndpointPrefixName + endpointTypeName;
    }

    private static bool IsEndpointMethod(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.MethodKind is MethodKind.Constructor or MethodKind.StaticConstructor)
        {
            return false;
        }

        if (methodSymbol.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet)
        {
            return false;
        }

        if (methodSymbol.Parameters.Length != 2)
        {
            return false;
        }

        if (methodSymbol.Parameters[1].Type.IsType("System.Threading", "CancellationToken") is false)
        {
            return false;
        }

        if (methodSymbol.ReturnsVoid)
        {
            return false;
        }

        return methodSymbol.ReturnType.GetTaskType()?.IsResultType() is true;
    }

    private static ITypeSymbol? GetTaskType(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsAnyType("System.Threading.Tasks", "Task", "ValueTask") is false)
        {
            return null;
        }

        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return null;
        }

        if (namedTypeSymbol.TypeArguments.Length is not 1)
        {
            return null;
        }

        return namedTypeSymbol.TypeArguments[0];
    }

    private static bool IsResultType(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsSystemType("Result") is false)
        {
            return false;
        }

        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return false;
        }

        if (namedTypeSymbol.TypeArguments.Length is not 2)
        {
            return false;
        }

        if (namedTypeSymbol.TypeArguments[1].IsSystemType("Failure") is false)
        {
            return false;
        }

        var failureType = namedTypeSymbol.TypeArguments[1] as INamedTypeSymbol;
        return failureType?.TypeArguments.Length is 1;
    }

    private static INamedTypeSymbol GetTypeByMetadataNameOrThrow(this Compilation compilation, string fullyQualifiedMetadataName)
        =>
        compilation.GetTypeByMetadataName(fullyQualifiedMetadataName)
            ?? throw new InvalidOperationException($"Interface '{fullyQualifiedMetadataName}' was not found in compilation");
}