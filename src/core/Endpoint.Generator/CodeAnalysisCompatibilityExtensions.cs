using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using PrimeFuncPack;

namespace GarageGroup.Infra;

internal static class CodeAnalysisCompatibilityExtensions
{
    internal static SourceBuilder AppendCodeLine(this SourceBuilder builder, params string[] codeLines)
        =>
        builder.AppendCodeLines(codeLines);

    internal static SourceBuilder AddUsings(this SourceBuilder builder, IEnumerable<string?>? namespaces)
        =>
        builder.AddUsing(namespaces?.Where(NotNullOrEmpty).Select(GetValue).ToArray() ?? []);

    internal static SourceBuilder EndCodeBlock(this SourceBuilder builder, char afterSymbol)
        =>
        builder.EndCodeBlock(afterSymbol.ToString());

    internal static SourceBuilder EndCollectionExpression(this SourceBuilder builder, char afterSymbol)
        =>
        builder.EndCollectionExpression(afterSymbol.ToString());

    internal static object? GetAttributeValue(this AttributeData attributeData, int constructorArgumentOrder)
    {
        if (constructorArgumentOrder < 0 || constructorArgumentOrder >= attributeData.ConstructorArguments.Length)
        {
            return null;
        }

        return attributeData.ConstructorArguments[constructorArgumentOrder].Value;
    }

    internal static object? GetAttributeValue(this AttributeData attributeData, int constructorArgumentOrder, string propertyName)
        =>
        attributeData.GetAttributeValue(constructorArgumentOrder) ?? attributeData.GetAttributePropertyValue(propertyName);

    internal static object? GetAttributePropertyValue(this AttributeData attributeData, string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return null;
        }

        foreach (var namedArgument in attributeData.NamedArguments)
        {
            if (string.Equals(namedArgument.Key, propertyName, System.StringComparison.Ordinal))
            {
                return namedArgument.Value.Value;
            }
        }

        return null;
    }

    internal static bool IsSystemType(this ITypeSymbol typeSymbol, string typeName)
        =>
        typeSymbol.IsType("System", typeName);

    internal static bool IsAnySystemType(this ITypeSymbol typeSymbol, params string[] typeNames)
        =>
        typeSymbol.IsAnyType("System", typeNames);

    internal static bool IsStreamType(this ITypeSymbol typeSymbol)
        =>
        typeSymbol.IsType("System.IO", "Stream");

    internal static ITypeSymbol? GetEnumUnderlyingTypeOrDefault(this ITypeSymbol typeSymbol)
        =>
        (typeSymbol as INamedTypeSymbol)?.EnumUnderlyingType;

    private static bool NotNullOrEmpty(string? source)
        =>
        string.IsNullOrEmpty(source) is false;

    private static string GetValue(string? source)
        =>
        source!;
}