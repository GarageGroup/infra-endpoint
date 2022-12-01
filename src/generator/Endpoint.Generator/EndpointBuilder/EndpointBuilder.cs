using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

using static EndpointAttributeHelper;

internal static partial class EndpointBuilder
{
    private const int MaxRecursiveSchemaLevel = 7;

    private const string DefaultSuccessStatusCodeValue = "200";

    private const string NoContentStatusCodeValue = "204";

    private static string GetDefaultStatusCode(this EndpointTypeDescription type)
        =>
        type.HasResponseBody() ? DefaultSuccessStatusCodeValue : NoContentStatusCodeValue;

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

    private static bool HasRequestBody(this EndpointTypeDescription type)
    {
        return type.RequestType?.GetConstructor()?.Parameters.Any(IsBodyInParameter) is true;

        static bool IsBodyInParameter(IParameterSymbol parameterSymbol)
            =>
            parameterSymbol.GetAttributes().Any(IsBodyInAttribute);

        static bool IsBodyInAttribute(AttributeData attributeData)
            =>
            IsJsonBodyInAttribute(attributeData) || IsRootBodyInAttribute(attributeData);
    }

    private static BodyTypeDescription? GetRequestBodyType(this EndpointTypeDescription type)
    {
        var constructorParameters = type.RequestType?.GetConstructor()?.Parameters;
        var bodyParameters = constructorParameters?.Where(IsRootBodyInParameter).ToArray();

        if (bodyParameters?.Length is not > 0)
        {
            return null;
        }

        if (bodyParameters.Length > 1 || constructorParameters?.Any(IsJsonBodyInParameter) is true)
        {
            throw new InvalidOperationException("There must be only one request body parameter");
        }

        var attributeData = bodyParameters[0].GetAttributes().FirstOrDefault(IsRootBodyInAttribute);

        return new(
            propertyName: bodyParameters[0].Name,
            propertySymbol: bodyParameters[0],
            contentType: attributeData?.GetAttributeValue(0)?.ToString(),
            bodyType: bodyParameters[0].Type);

        static bool IsRootBodyInParameter(IParameterSymbol parameterSymbol)
            =>
            parameterSymbol.GetAttributes().Any(IsRootBodyInAttribute);

        static bool IsJsonBodyInParameter(IParameterSymbol parameterSymbol)
            =>
            parameterSymbol.GetAttributes().Any(IsJsonBodyInAttribute);
    }

    private static IReadOnlyCollection<JsonBodyPropertyDescription> GetRequestJsonBodyProperties(this EndpointTypeDescription type)
    {
        var constructorParameters = type.RequestType?.GetConstructor()?.Parameters;
        if (constructorParameters?.Length is not > 0)
        {
            return Array.Empty<JsonBodyPropertyDescription>();
        }

        return InnerGetProperties().ToArray();

        IEnumerable<JsonBodyPropertyDescription> InnerGetProperties()
        {
            foreach (var parameter in constructorParameters)
            {
                var attributeData = parameter.GetAttributes().FirstOrDefault(IsJsonBodyInAttribute);
                if (attributeData is null)
                {
                    continue;
                }

                var propertyName = attributeData.GetAttributeValue(0)?.ToString();

                yield return new(
                    propertyName: parameter.Name,
                    propertySymbol: parameter,
                    jsonPropertyName: string.IsNullOrEmpty(propertyName) ? parameter.Name : propertyName!,
                    propertyType: parameter.Type);
            }
        }
    }

    private static bool HasResponseBody(this EndpointTypeDescription type)
    {
        return type.ResponseType?.GetPublicReadableProperties().Any(IsBodyOutProperty) is true;

        static bool IsBodyOutProperty(IPropertySymbol propertySymbol)
            =>
            propertySymbol.GetAttributes().Any(IsBodyOutAttribute);

        static bool IsBodyOutAttribute(AttributeData attributeData)
            =>
            IsJsonBodyOutAttribute(attributeData) || IsRootBodyOutAttribute(attributeData);
    }

    private static BodyTypeDescription? GetResponseBodyType(this EndpointTypeDescription type)
    {
        var properties = type.ResponseType?.GetPublicReadableProperties();
        var bodyProperties = properties?.Where(IsBodyOutProperty).ToArray();

        if (bodyProperties?.Length is not > 0)
        {
            return null;
        }

        if (bodyProperties.Length is not 1 || properties.Any(IsJsonBodyOutProperty))
        {
            throw new InvalidOperationException("There must be only one response body parameter");
        }

        var attributeData = bodyProperties[0].GetAttributes().FirstOrDefault(IsRootBodyOutAttribute);

        return new(
            propertyName: bodyProperties[0].Name,
            propertySymbol: bodyProperties[0],
            contentType: attributeData?.GetAttributeValue(0)?.ToString(),
            bodyType: bodyProperties[0].Type);

        static bool IsBodyOutProperty(IPropertySymbol propertySymbol)
            =>
            propertySymbol.GetAttributes().Any(IsRootBodyOutAttribute);

        static bool IsJsonBodyOutProperty(IPropertySymbol propertySymbol)
            =>
            propertySymbol.GetAttributes().Any(IsJsonBodyOutAttribute);
    }

    private static IReadOnlyCollection<JsonBodyPropertyDescription> GetResponseJsonBodyProperties(this EndpointTypeDescription type)
    {
        var properties = type.ResponseType?.GetPublicReadableProperties();
        if (properties is null)
        {
            return Array.Empty<JsonBodyPropertyDescription>();
        }

        return InnerGetProperties().ToArray();

        IEnumerable<JsonBodyPropertyDescription> InnerGetProperties()
        {
            foreach (var property in properties)
            {
                var attributeData = property.GetAttributes().FirstOrDefault(IsJsonBodyOutAttribute);
                if (attributeData is null)
                {
                    continue;
                }

                var propertyName = attributeData.GetAttributeValue(0)?.ToString();

                yield return new(
                    propertyName: property.Name,
                    propertySymbol: property,
                    jsonPropertyName: string.IsNullOrEmpty(propertyName) ? property.GetJsonPropertyName() : propertyName!,
                    propertyType: property.Type);
            }
        }
    }

    private static IEnumerable<IPropertySymbol> GetPublicReadableProperties(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().OfType<IPropertySymbol>().Where(IsPublic).Where(IsReadable);

        static bool IsPublic(IPropertySymbol propertySymbol)
            =>
            propertySymbol.DeclaredAccessibility is Accessibility.Public;

        static bool IsReadable(IPropertySymbol propertySymbol)
            =>
            propertySymbol.GetMethod is not null;
    }

    private static IReadOnlyCollection<ProblemData> GetProblemData(this ITypeSymbol typeSymbol)
    {
        return InnerGetProblemData().Where(NotEmptyStatusCode).ToArray();

        IEnumerable<ProblemData> InnerGetProblemData()
        {
            foreach (var enumField in typeSymbol.GetEnumFields())
            {
                var problemAttribute = enumField.GetAttributes().FirstOrDefault(IsProblemAttribute);
                if (problemAttribute is null)
                {
                    continue;
                }

                yield return new(
                    statusFieldName: enumField.Name,
                    statusCode: problemAttribute?.GetAttributeValue(0, "StatusCode")?.ToString(),
                    detail: problemAttribute?.GetAttributeValue(1, "Detail")?.ToString(),
                    title: problemAttribute?.GetAttributeValue(2, "Title")?.ToString());
            }
        }

        static bool IsProblemAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsType("GGroupp.Infra", "ProblemAttribute") is true;

        static bool NotEmptyStatusCode(ProblemData? problemData)
            =>
            string.IsNullOrEmpty(problemData?.StatusCode) is false;
    }

    private static ITypeSymbol? GetSuccessStatusCodeType(this EndpointTypeDescription type)
    {
        return type.ResponseType?.AllInterfaces.Where(IsSuccessStatusCodeProvider).FirstOrDefault(HasOneTypeArgument)?.TypeArguments[0];

        static bool IsSuccessStatusCodeProvider(INamedTypeSymbol namedTypeSymbol)
            =>
            namedTypeSymbol.IsType("GGroupp.Infra", "ISuccessStatusCodeProvider");

        static bool HasOneTypeArgument(INamedTypeSymbol namedTypeSymbol)
            =>
            namedTypeSymbol.TypeArguments.Length is 1;
    }

    private static bool IsNullable(this ITypeSymbol typeSymbol)
        =>
        typeSymbol.GetNullableStructType() is not null;

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
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            return arrayTypeSymbol.ElementType;
        }

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

    private static string GetStatusDescription(string? successStatusCode)
        =>
        successStatusCode switch
        {
            "200"   => "Success",
            "201"   => "Created",
            "202"   => "Accepted",
            "203"   => "NonAuthoritativeInformation",
            "204"   => "NoContent",
            "205"   => "ResetContent",
            "206"   => "PartialContent",
            "207"   => "MultiStatus",
            "208"   => "AlreadyReported",
            "400"   => "BadRequest",
            "401"   => "Unauthorized",
            "402"   => "PaymentRequired",
            "403"   => "Forbidden",
            "404"   => "NotFound",
            "406"   => "NotAcceptable",
            "408"   => "RequestTimeout",
            "409"   => "Conflict",
            "410"   => "Gone",
            "411"   => "LengthRequired",
            "412"   => "PreconditionFailed",
            "416"   => "RequestedRangeNotSatisfiable",
            "417"   => "ExpectationFailed",
            "422"   => "UnprocessableEntity",
            "423"   => "Locked",
            "429"   => "TooManyRequests",
            "500"   => "InternalServerError",
            _       => successStatusCode ?? string.Empty
        };

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