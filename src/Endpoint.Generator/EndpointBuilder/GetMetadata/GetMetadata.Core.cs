using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

using static EndpointAttributeHelper;

partial class EndpointBuilder
{
    private static OperationParameterDescription? GetOperationParameterDescription(IParameterSymbol parameterSymbol)
    {
        if (parameterSymbol.GetAttributes().Any(IsRootBodyInAttribute))
        {
            return null;
        }

        if (parameterSymbol.GetAttributes().Any(IsJsonBodyInAttribute))
        {
            return null;
        }

        if (parameterSymbol.GetAttributes().Any(IsClaimInAttribute))
        {
            return null;
        }

        var parameterName = parameterSymbol.Name;
        var isNullable = parameterSymbol.Type.IsNullable() || parameterSymbol.NullableAnnotation is NullableAnnotation.Annotated;
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

    private static string? GetSimpleSchemaFunction(this ITypeSymbol typeSymbol, bool? nullable = null)
    {
        var isNullable = nullable is not null ? nullable.Value : typeSymbol.IsNullable();
        var type = typeSymbol.GetNullableStructType() ?? typeSymbol;

        if (type.IsSystemType(nameof(String)))
        {
            return $"CreateStringSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType(nameof(Guid)))
        {
            return $"CreateUuidSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType("DateOnly"))
        {
            return $"CreateDateSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType(nameof(DateTime)) || typeSymbol.IsSystemType(nameof(DateTimeOffset)))
        {
            return $"CreateDateTimeSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType(nameof(Boolean)))
        {
            return $"CreateBooleanSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType(nameof(Int32)))
        {
            return $"CreateInt32Schema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType(nameof(Int64)))
        {
            return $"CreateInt64Schema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType(nameof(Int16)) || typeSymbol.IsSystemType("Byte"))
        {
            return $"CreateIntegerSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType(nameof(Double)))
        {
            return $"CreateDoubleSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType(nameof(Single)))
        {
            return $"CreateFloatSchema({isNullable.ToStringValue()})";
        }

        if (type.IsSystemType(nameof(Decimal)))
        {
            return $"CreateNumberSchema({isNullable.ToStringValue()})";
        }

        if (type.GetEnumUnderlyingType() is INamedTypeSymbol enumUnderlyingType)
        {
            return GetSimpleSchemaFunction(enumUnderlyingType, isNullable);
        }

        if (type.IsStreamType())
        {
            return $"CreateBinarySchema({isNullable.ToStringValue()})";
        }

        if (typeSymbol.GetCollectionType()?.IsSystemType(nameof(Byte)) is true)
        {
            return $"CreateByteSchema({isNullable.ToStringValue()})";
        }

        return null;
    }

    private static IReadOnlyCollection<string> GetSuccessStatusCodes(this EndpointTypeDescription type)
    {
        var successStatusType = type.GetSuccessStatusCodeType();
        if (successStatusType is null)
        {
            var successStatusCode = GetSuccessStatusCode(type.ResponseType?.GetAttributes().FirstOrDefault(IsSuccessAttribute));
            if (string.IsNullOrEmpty(successStatusCode) is false)
            {
                return new[] { successStatusCode! };
            }

            return Array.Empty<string>();
        }

        return successStatusType.GetEnumFields().Select(GetSuccessAttribute).Select(GetSuccessStatusCode).Where(NotEmpty).Distinct().ToArray()!;

        static AttributeData? GetSuccessAttribute(ISymbol symbol)
            =>
            symbol.GetAttributes().FirstOrDefault(IsSuccessAttribute);

        static bool NotEmpty(string? statusCode)
            =>
            string.IsNullOrEmpty(statusCode) is false;

        static string? GetSuccessStatusCode(AttributeData? attributeData)
            =>
            attributeData?.GetAttributeValue(0, "StatusCode")?.ToString();
    }
}