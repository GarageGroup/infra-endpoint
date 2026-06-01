using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using PrimeFuncPack;

namespace GarageGroup.Infra;

internal static partial class SourceGeneratorExtensions
{
    private const string EndpointAttributeName = "GarageGroup.Infra.EndpointAttribute";

    private const string EndpointSetAttributeName = "GarageGroup.Infra.EndpointSetAttribute";

    private static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source)
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

    private static string GetTypeRootName(this string endpointTypeName)
    {
        if (string.IsNullOrEmpty(endpointTypeName))
        {
            throw new InvalidOperationException("Endpoint type name must be specified");
        }

        if (endpointTypeName.Length > 1 && endpointTypeName.StartsWith("I", StringComparison.InvariantCultureIgnoreCase))
        {
            endpointTypeName = endpointTypeName.Substring(1);
        }

        endpointTypeName = RemoveFromFirstIndex(endpointTypeName, "HttpFunc");
        endpointTypeName = RemoveFromFirstIndex(endpointTypeName, "Func");
        endpointTypeName = RemoveLastSuffix(endpointTypeName, "EndpointApi");
        endpointTypeName = RemoveLastSuffix(endpointTypeName, "Api");

        return endpointTypeName;

        static string RemoveFromFirstIndex(string source, string value)
        {
            var index = source.IndexOf(value, StringComparison.InvariantCultureIgnoreCase);
            return index > 0 ? source.Substring(0, index) : source;
        }

        static string RemoveLastSuffix(string source, string value)
        {
            if (source.Length <= value.Length || source.EndsWith(value, StringComparison.InvariantCultureIgnoreCase) is false)
            {
                return source;
            }

            return source.Substring(0, source.Length - value.Length);
        }
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

        if (methodSymbol.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal))
        {
            return false;
        }

        if (methodSymbol.Parameters.Length is not 2)
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

        return methodSymbol.ReturnType.GetTaskType() is not null;
    }

    private static INamedTypeSymbol? GetTaskType(this ITypeSymbol typeSymbol)
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

        return namedTypeSymbol.TypeArguments[0] as INamedTypeSymbol;
    }

    private static bool IsResultType(this ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null)
        {
            return false;
        }

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
