using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

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
        var description = parameterSymbol.GetDescriptionValue();
        var parameterType = parameterSymbol.Type;
    
        var schemaFunction = parameterType.GetSimpleSchemaFunction(usings, exmapleValue, description)
            ?? parameterType.GetArrayOrDefaultSchemaFunction(usings, exmapleValue, description);

        if (parameterSymbol.GetAttributes().FirstOrDefault(IsRouteInAttribute) is AttributeData routeInAttribute)
        {
            var name = routeInAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();

            return new(
                required: true,
                location: "Path",
                name: string.IsNullOrWhiteSpace(name) ? parameterName : name!,
                schemaFunction: schemaFunction,
                description: routeInAttribute.GetAttributePropertyValue("Description")?.ToString());
        }
        
        if (parameterSymbol.GetAttributes().FirstOrDefault(IsQueryInAttribute) is AttributeData queryInAttribute)
        {
            var name = queryInAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();

            return new(
                required: isNullable is false,
                location: "Query",
                name: string.IsNullOrWhiteSpace(name) ? parameterName : name!,
                schemaFunction: schemaFunction,
                description: queryInAttribute.GetAttributePropertyValue("Description")?.ToString());
        }

        if (parameterSymbol.GetAttributes().FirstOrDefault(IsHeaderInAttribute) is AttributeData headerInAttribute)
        {
            var name = headerInAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();

            return new(
                required: isNullable is false,
                location: "Header",
                name: string.IsNullOrWhiteSpace(name) ? parameterName : name!,
                schemaFunction: schemaFunction,
                description: headerInAttribute.GetAttributePropertyValue("Description")?.ToString());
        }

        return new(
            required: true,
            location: "Query",
            name: parameterName,
            schemaFunction: schemaFunction,
            description: null);
    }

    private static string GetArrayOrDefaultSchemaFunction(
        this ITypeSymbol typeSymbol, List<string> usings, string? exmapleValue, string? description)
    {
        var isNullable = typeSymbol.IsNullable();
        var collectionType = typeSymbol.GetCollectionTypeOrDefault();

        if (collectionType is null)
        {
            return BuildSchemaFunction($"CreateDefaultSchema", usings, exmapleValue, description, isNullable);
        }

        var collectionTypeSchemaFunction = collectionType.GetSimpleSchemaFunction(usings, exmapleValue, description);
        if (string.IsNullOrEmpty(collectionTypeSchemaFunction) is false)
        {
            return $"CreateArraySchema({isNullable.ToStringValue()}, {collectionTypeSchemaFunction})";
        }

        var defaultSchemaFunction = BuildSchemaFunction($"CreateDefaultSchema", usings, exmapleValue, description, isNullable);
        return $"CreateArraySchema({isNullable.ToStringValue()}, {defaultSchemaFunction})";
    }

    private static string? GetSimpleSchemaFunction(
        this ITypeSymbol typeSymbol, List<string> usings, string? exmapleValue, string? description, bool? nullable = null)
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

        if (type.GetEnumUnderlyingTypeOrDefault() is not null)
        {
            var typeData = type.GetDisplayedData();
            usings.AddRange(typeData.AllNamespaces);

            return InnerBuildSchemaFunction($"CreateEnumSchema<{typeData.DisplayedTypeName}>");
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
            BuildSchemaFunction(functionName, usings, exmapleValue, description, isNullable);
    }

    private static string BuildSchemaFunction(
        string functionName, List<string> usings, string? exmapleValue, string? description, bool isNullable)
    {
        if (string.IsNullOrEmpty(exmapleValue) is false)
        {
            usings.Add("Microsoft.OpenApi.Any");
        }
        else
        {
            exmapleValue = "default";
        }

        if (string.IsNullOrEmpty(description))
        {
            description = "default";
        }

        return $"{functionName}({isNullable.ToStringValue()}, example: {exmapleValue}, description: {description})";
    }

    private static string? GetExampleValue(this ISymbol symbol)
    {
        var stringExampleAttribute = symbol.GetAttributes().FirstOrDefault(IsStringExampleAttribute);
        if (stringExampleAttribute is null)
        {
            return null;
        }

        var value = stringExampleAttribute.GetAttributeValue(0)?.ToString();
        return $"new OpenApiString({value.AsStringSourceCodeOrStringEmpty()})";
    }

    private static string? GetDescriptionValue(this ISymbol symbol)
    {
        var swaggerDescriptionAttribute = symbol.GetAttributes().FirstOrDefault(IsSwaggerDescriptionAttribute);
        if (swaggerDescriptionAttribute is null)
        {
            return null;
        }

        var value = swaggerDescriptionAttribute.GetAttributeValue(0)?.ToString();
        return value.AsStringValueOrDefault();
    }

    private static IReadOnlyCollection<SuccessData> GetSuccessData(this EndpointTypeDescription type)
    {
        var successStatusType = type.GetSuccessStatusCodeType();
        if (successStatusType is null)
        {
            var successData = GetSuccessData(type.ResponseType?.GetAttributes().FirstOrDefault(IsSuccessAttribute));
            if (successData is not null)
            {
                return new[] { successData };
            }

            return Array.Empty<SuccessData>();
        }

        return successStatusType.GetEnumFields().Select(GetSuccessAttribute).Select(GetSuccessData).ToArray();

        static AttributeData? GetSuccessAttribute(ISymbol symbol)
            =>
            symbol.GetAttributes().FirstOrDefault(IsSuccessAttribute);

        static SuccessData GetSuccessData(AttributeData? attributeData)
            =>
            new(
                statusCode: attributeData?.GetAttributeValue(0, "StatusCode")?.ToString(),
                description: attributeData?.GetAttributePropertyValue("Description")?.ToString());
    }
}