using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

using static EndpointAttributeHelper;

internal static partial class EndpointBuilder
{
    private static IMethodSymbol? GetConstructor(this ITypeSymbol typeSymbol)
    {
        var methods = typeSymbol.GetMembers().OfType<IMethodSymbol>();

        return methods.Where(IsPublic).Where(IsConstructor).OrderByDescending(GetParametersLength).FirstOrDefault();

        static bool IsConstructor(IMethodSymbol methodSymbol)
            =>
            methodSymbol.MethodKind is MethodKind.Constructor;

        static int GetParametersLength(IMethodSymbol methodSymbol)
            =>
            methodSymbol.Parameters.Length;
    }

    private static bool IsBodyParameter(IParameterSymbol parameterSymbol)
        =>
        parameterSymbol.GetAttributes().Any(IsBodyInAttribute);

    private static OperationParameterDescription? GetOperationParameterDescription(IParameterSymbol parameterSymbol)
    {
        if (parameterSymbol.GetAttributes().Any(IsBodyInAttribute) || parameterSymbol.GetAttributes().Any(IsClaimInAttribute))
        {
            return null;
        }

        var parameterName = parameterSymbol.Name;
        var isNullable = parameterSymbol.Type.IsNullable();
        var schemaFunction = parameterSymbol.Type.GetSimpleSchemaFunction() ?? parameterSymbol.Type.GetArrayOrDefaultSchemaFunction();

        if (parameterSymbol.GetAttributes().FirstOrDefault(IsRouteInAttribute) is AttributeData routeInAttribute)
        {
            var name = routeInAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();

            return new(
                required: true,
                location: "Path",
                name: string.IsNullOrWhiteSpace(name) ? parameterName : name!,
                schemaFunction: schemaFunction);
        }
        
        if (parameterSymbol.GetAttributes().FirstOrDefault(IsQueryInAttribute) is AttributeData queryInAttribute)
        {
            var name = queryInAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();

            return new(
                required: isNullable is false,
                location: "Query",
                name: string.IsNullOrWhiteSpace(name) ? parameterName : name!,
                schemaFunction: schemaFunction);
        }

        if (parameterSymbol.GetAttributes().FirstOrDefault(IsHeaderInAttribute) is AttributeData headerInAttribute)
        {
            var name = headerInAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();

            return new(
                required: isNullable is false,
                location: "Header",
                name: string.IsNullOrWhiteSpace(name) ? parameterName : name!,
                schemaFunction: schemaFunction);
        }

        return new(
            required: true,
            location: "Query",
            name: parameterName,
            schemaFunction: schemaFunction);
    }

    private static bool IsNullable(this ITypeSymbol typeSymbol)
        =>
        typeSymbol.GetNullableStructType() is not null;

    private static string GetArrayOrDefaultSchemaFunction(this ITypeSymbol typeSymbol)
    {
        var isNullable = typeSymbol.IsNullable();
        var collectionType = typeSymbol.GetCollectionType();

        if (collectionType is null)
        {
            return $"CreateDefaultSchema({isNullable.ToStringValue()})";
        }

        var collectionTypeSchemaFunction = collectionType.GetSimpleSchemaFunction();
        if (string.IsNullOrEmpty(collectionTypeSchemaFunction) is false)
        {
            return $"CreateArraySchema({isNullable.ToStringValue()}, {collectionTypeSchemaFunction})";
        }

        return $"CreateArraySchema({isNullable.ToStringValue()}, CreateDefaultSchema({isNullable.ToStringValue()}))";
    }

    private static string? GetSimpleSchemaFunction(this ITypeSymbol typeSymbol)
    {
        var isNullable = typeSymbol.IsNullable();
        var type = typeSymbol.GetNullableStructType() ?? typeSymbol;

        if (type.IsSystemType("String"))
        {
            return $"CreateStringSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("Guid"))
        {
            return $"CreateUuidSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("DateOnly"))
        {
            return $"CreateDateSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("DateTime") || typeSymbol.IsSystemType("DateTimeOffset"))
        {
            return $"CreateDateTimeSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("Boolean"))
        {
            return $"CreateBooleanSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("Int32"))
        {
            return $"CreateInt32Schema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("Int64"))
        {
            return $"CreateInt64Schema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("Int16") || typeSymbol.IsSystemType("Byte"))
        {
            return $"CreateIntegerSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("Double"))
        {
            return $"CreateDoubleSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("Single"))
        {
            return $"CreateFloatSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("Decimal"))
        {
            return $"CreateNumberSchema({isNullable.ToStringValue()})";
        }

        if (type.IsType("System.IO", "Stream"))
        {
            return $"CreateBinarySchema({isNullable.ToStringValue()})";
        }

        if (typeSymbol.GetCollectionType()?.IsSystemType("Byte") is true)
        {
            return $"CreateByteSchema({isNullable.ToStringValue()})";
        }

        return null;
    }

    private static ITypeSymbol? GetNullableStructType(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsValueType is false)
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

        if (namedTypeSymbol.IsSystemType("Nullable") is false)
        {
            return null;
        }

        return namedTypeSymbol.TypeArguments[0];
    }

    private static ITypeSymbol? GetCollectionType(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return null;
        }

        return namedTypeSymbol.AllInterfaces.FirstOrDefault(IsEnumerable)?.TypeArguments[0];

        static bool IsEnumerable(INamedTypeSymbol symbol)
            =>
            symbol.IsType("System.Collections.Generic", "IEnumerable") && symbol.TypeArguments.Length is 1;
    }

    private static bool IsPublic(this IMethodSymbol methodSymbol)
        =>
        methodSymbol.DeclaredAccessibility is Accessibility.Public;

    private static string ToStringValue(this bool source)
        =>
        source ? "true" : "false";

    private static string ToStringValueOrDefault(this string? source)
        =>
        string.IsNullOrEmpty(source) ? "default" : $"\"{source.EncodeString()}\"";

    private static string ToStringValueOrEmpty(this string? source)
        =>
        string.IsNullOrEmpty(source) ? "string.Empty" : $"\"{source.EncodeString()}\"";

    private static string? EncodeString(this string? source)
        =>
        source?.Replace("\"", "\\\"");
}