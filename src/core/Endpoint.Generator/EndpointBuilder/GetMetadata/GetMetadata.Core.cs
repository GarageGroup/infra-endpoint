using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

using static EndpointAttributeHelper;

partial class EndpointBuilder
{
    private static OperationParameterDescription? GetOperationParameterDescription(IParameterSymbol parameterSymbol, List<string> usings)
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

        var exmapleValue = parameterSymbol.GetExampleValue();
        var parameterType = parameterSymbol.Type;
    
        var schemaFunction = parameterType.GetSimpleSchemaFunction(usings, exmapleValue)
            ?? parameterType.GetArrayOrDefaultSchemaFunction(usings, exmapleValue);

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

    private static string GetArrayOrDefaultSchemaFunction(this ITypeSymbol typeSymbol, List<string> usings, string? exmapleValue)
    {
        var isNullable = typeSymbol.IsNullable();
        var collectionType = typeSymbol.GetCollectionTypeOrDefault();

        if (collectionType is null)
        {
            return BuildSchemaFunction($"CreateDefaultSchema", usings, exmapleValue, isNullable);
        }

        var collectionTypeSchemaFunction = collectionType.GetSimpleSchemaFunction(usings, exmapleValue);
        if (string.IsNullOrEmpty(collectionTypeSchemaFunction) is false)
        {
            return $"CreateArraySchema({isNullable.ToStringValue()}, {collectionTypeSchemaFunction})";
        }

        var defaultSchemaFunction = BuildSchemaFunction($"CreateDefaultSchema", usings, exmapleValue, isNullable);
        return $"CreateArraySchema({isNullable.ToStringValue()}, {defaultSchemaFunction})";
    }

    private static string? GetSimpleSchemaFunction(this ITypeSymbol typeSymbol, List<string> usings, string? exmapleValue, bool? nullable = null)
    {
        var isNullable = nullable is not null ? nullable.Value : typeSymbol.IsNullable();
        var type = typeSymbol.GetNullableStructType() ?? typeSymbol;

        if (type.IsSystemType(nameof(String)))
        {
            return InnerBuildSchemaFunction("CreateStringSchema");
        }

        if (type.IsSystemType(nameof(Guid)))
        {
            return InnerBuildSchemaFunction("CreateUuidSchema");
        }

        if (type.IsSystemType("DateOnly"))
        {
            return InnerBuildSchemaFunction("CreateDateSchema");
        }

        if (type.IsSystemType(nameof(DateTime)) || type.IsSystemType(nameof(DateTimeOffset)))
        {
            return InnerBuildSchemaFunction("CreateDateTimeSchema");
        }

        if (type.IsSystemType(nameof(TimeSpan)))
        {
            return InnerBuildSchemaFunction("CreateTimeSpanSchema");
        }

        if (type.IsSystemType(nameof(Boolean)))
        {
            return InnerBuildSchemaFunction("CreateBooleanSchema");
        }

        if (type.IsSystemType(nameof(Int32)))
        {
            return InnerBuildSchemaFunction("CreateInt32Schema");
        }

        if (type.IsSystemType(nameof(Int64)))
        {
            return InnerBuildSchemaFunction("CreateInt64Schema");
        }

        if (type.IsSystemType(nameof(Int16)) || type.IsSystemType("Byte"))
        {
            return InnerBuildSchemaFunction("CreateIntegerSchema");
        }

        if (type.IsSystemType(nameof(Double)))
        {
            return InnerBuildSchemaFunction("CreateDoubleSchema");
        }

        if (type.IsSystemType(nameof(Single)))
        {
            return InnerBuildSchemaFunction("CreateFloatSchema");
        }

        if (type.IsSystemType(nameof(Decimal)))
        {
            return InnerBuildSchemaFunction("CreateNumberSchema");
        }

        if (type.GetEnumUnderlyingTypeOrDefault() is INamedTypeSymbol enumUnderlyingType)
        {
            return enumUnderlyingType.GetSimpleSchemaFunction(usings, exmapleValue, isNullable);
        }

        if (type.IsStreamType())
        {
            return InnerBuildSchemaFunction("CreateBinarySchema");
        }

        if (type.GetCollectionTypeOrDefault()?.IsSystemType(nameof(Byte)) is true)
        {
            return InnerBuildSchemaFunction("CreateByteSchema");
        }

        if (type.IsSchemaProviderType())
        {
            var typeData = type.GetDisplayedData();
            usings.AddRange(typeData.AllNamespaces);

            return InnerBuildSchemaFunction($"{typeData.DisplayedTypeName}.GetSchema");
        }

        return null;

        string InnerBuildSchemaFunction(string functionName)
            =>
            BuildSchemaFunction(functionName, usings, exmapleValue, isNullable);
    }

    private static string BuildSchemaFunction(string functionName, List<string> usings, string? exmapleValue, bool isNullable)
    {
        if (string.IsNullOrEmpty(exmapleValue))
        {
            return $"{functionName}({isNullable.ToStringValue()})";
        }

        usings.Add("Microsoft.OpenApi.Any");
        return $"{functionName}({isNullable.ToStringValue()}, {exmapleValue})";
    }

    private static string? GetExampleValue(this ISymbol symbol)
    {
        var stringExampleAttribute = symbol.GetAttributes().FirstOrDefault(IsStringExampleAttribute);
        if (stringExampleAttribute is null)
        {
            return null;
        }

        var value = stringExampleAttribute.GetAttributeValue(0)?.ToString();
        return $"new OpenApiString({value.ToStringValueOrEmpty()})";
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