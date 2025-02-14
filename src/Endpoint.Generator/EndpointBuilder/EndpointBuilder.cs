using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

using static EndpointAttributeHelper;

internal static partial class EndpointBuilder
{
    private const int MaxRecursiveSchemaLevel = 10;

    private const string DefaultSuccessStatusCodeValue = "200";

    private const string NoContentStatusCodeValue = "204";

    private const string MediaTypePropertyName = "MediaType";

    private static SourceBuilder AppendObsoleteAttributeIfNecessary(this SourceBuilder builder, EndpointTypeDescription type)
    {
        if (type.ObsoleteData is null)
        {
            return builder;
        }

        var attributeBuilder = new StringBuilder("[Obsolete(").Append(type.ObsoleteData.Message.AsStringValueOrDefault());

        attributeBuilder = type.ObsoleteData.IsError switch
        {
            true => attributeBuilder.Append(", true"),
            false => attributeBuilder.Append(", false"),
            _ => attributeBuilder
        };

        if (string.IsNullOrEmpty(type.ObsoleteData.DiagnosticId) is false)
        {
            attributeBuilder = attributeBuilder.Append(", DiagnosticId = ").Append(type.ObsoleteData.DiagnosticId.AsStringValueOrDefault());
        }

        if (string.IsNullOrEmpty(type.ObsoleteData.UrlFormat) is false)
        {
            attributeBuilder = attributeBuilder.Append(", UrlFormat = ").Append(type.ObsoleteData.UrlFormat.AsStringValueOrDefault());
        }

        attributeBuilder = attributeBuilder.Append(")]");
        return builder.AppendCodeLine(attributeBuilder.ToString());
    }

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
            IsJsonBodyInAttribute(attributeData) || IsFormBodyInAttribute(attributeData) || IsRootBodyInAttribute(attributeData);
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

        var rootBodyAttribute = bodyParameters[0].GetAttributes().FirstOrDefault(IsRootBodyInAttribute);

        return new(
            propertyName: bodyParameters[0].Name,
            propertySymbol: bodyParameters[0],
            contentType: new(rootBodyAttribute?.GetAttributeValue(0)?.ToString()),
            bodyType: bodyParameters[0].Type);

        static bool IsRootBodyInParameter(IParameterSymbol parameterSymbol)
            =>
            parameterSymbol.GetAttributes().Any(IsRootBodyInAttribute);

        static bool IsJsonBodyInParameter(IParameterSymbol parameterSymbol)
            =>
            parameterSymbol.GetAttributes().Any(IsJsonBodyInAttribute);
    }

    private static IReadOnlyCollection<BodyPropertyDescription> GetRequestBodyProperties(this EndpointTypeDescription type)
    {
        var constructorParameters = type.RequestType?.GetConstructor()?.Parameters;
        if (constructorParameters?.Length is not > 0)
        {
            return [];
        }

        var jsonProperties = InnerGetProperties(IsJsonBodyInAttribute, BodyPropertyKind.Json).ToArray();
        var formProperties = InnerGetProperties(IsFormBodyInAttribute, BodyPropertyKind.Form).ToArray();

        if (jsonProperties.Length > 0 && formProperties.Length > 0)
        {
            throw new InvalidOperationException("There must be only either json or form parameters in one request type");
        }

        return jsonProperties.Length > 0 ? jsonProperties : formProperties;

        IEnumerable<BodyPropertyDescription> InnerGetProperties(Func<AttributeData, bool> predicate, BodyPropertyKind propertyKind)
        {
            foreach (var parameter in constructorParameters)
            {
                var attributeData = parameter.GetAttributes().FirstOrDefault(predicate);
                if (attributeData is null)
                {
                    continue;
                }

                var propertyName = attributeData.GetAttributeValue(0)?.ToString();

                yield return new(
                    propertyName: parameter.Name,
                    bodyParameterName: string.IsNullOrEmpty(propertyName) ? parameter.Name : propertyName!,
                    propertySymbol: parameter,
                    propertyType: parameter.Type,
                    propertyKind: propertyKind);
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
            contentType: new(attributeData?.GetAttributeValue(0)?.ToString()),
            bodyType: bodyProperties[0].Type);

        static bool IsBodyOutProperty(IPropertySymbol propertySymbol)
            =>
            propertySymbol.GetAttributes().Any(IsRootBodyOutAttribute);

        static bool IsJsonBodyOutProperty(IPropertySymbol propertySymbol)
            =>
            propertySymbol.GetAttributes().Any(IsJsonBodyOutAttribute);
    }

    private static IReadOnlyCollection<BodyPropertyDescription> GetResponseBodyProperties(this EndpointTypeDescription type)
    {
        var properties = type.ResponseType?.GetPublicReadableProperties();
        if (properties is null)
        {
            return [];
        }

        return InnerGetProperties().ToArray();

        IEnumerable<BodyPropertyDescription> InnerGetProperties()
        {
            foreach (var property in properties)
            {
                var attributeData = property.GetAttributes().FirstOrDefault(IsJsonBodyOutAttribute);
                if (attributeData is null || property.GetJsonIgnoreCondition() is JsonIgnoreCondition.Always)
                {
                    continue;
                }

                var propertyName = attributeData.GetAttributeValue(0)?.ToString();

                yield return new(
                    propertyName: property.Name,
                    bodyParameterName: string.IsNullOrEmpty(propertyName) ? property.GetJsonPropertyName() : propertyName!,
                    propertySymbol: property,
                    propertyType: property.Type,
                    propertyKind: BodyPropertyKind.Json);
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

                string? detail = null;
                bool detailFromFailureMessage = false;

                var detailValue = problemAttribute.GetAttributeValue(1);
                if (detailValue is bool fromFailureMessage)
                {
                    detailFromFailureMessage = fromFailureMessage;
                }
                else
                {
                    detail = detailValue?.ToString();
                }

                yield return new(
                    statusFieldName: enumField.Name,
                    statusCode: problemAttribute.GetAttributeValue(0, "StatusCode")?.ToString(),
                    detail: detail,
                    detailFromFailureMessage: detailFromFailureMessage,
                    title: problemAttribute.GetAttributeValue(2, "Title")?.ToString(),
                    description: problemAttribute.GetAttributePropertyValue("Description")?.ToString());
            }
        }

        static bool IsProblemAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsType("GarageGroup.Infra", "ProblemAttribute") is true;

        static bool NotEmptyStatusCode(ProblemData? problemData)
            =>
            string.IsNullOrEmpty(problemData?.StatusCode) is false;
    }

    private static ITypeSymbol? GetSuccessStatusCodeType(this EndpointTypeDescription type)
    {
        return type.ResponseType?.AllInterfaces.Where(IsSuccessStatusCodeProvider).FirstOrDefault(HasOneTypeArgument)?.TypeArguments[0];

        static bool IsSuccessStatusCodeProvider(INamedTypeSymbol namedTypeSymbol)
            =>
            namedTypeSymbol.IsType("GarageGroup.Infra", "ISuccessStatusCodeProvider");

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

    private static bool IsPublic(this IMethodSymbol methodSymbol)
        =>
        methodSymbol.DeclaredAccessibility is Accessibility.Public;

    private static string GetMethodValue(this EndpointTypeDescription type)
        =>
        string.IsNullOrEmpty(type.MethodName) ? "default" : $"EndpointMethod.{type.MethodName}";

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
            "300"   => "Ambiguous",
            "302"   => "Redirect",
            "303"   => "RedirectMethod",
            "307"   => "RedirectKeepVerb",
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
            "426"   => "UpgradeRequired",
            "429"   => "TooManyRequests",
            "500"   => "InternalServerError",
            _       => successStatusCode ?? string.Empty
        };

    private static string ToStringValue(this bool source)
        =>
        source ? "true" : "false";

    private static string AsStringValueOrDefault(this string? source)
        =>
        source.AsStringSourceCodeOr("default");

    private static JsonIgnoreCondition GetJsonIgnoreCondition(this IPropertySymbol? property)
    {
        var jsonIgnoreAttribute = property?.GetAttributes().FirstOrDefault(IsJsonIgnoreAttribute);
        if (jsonIgnoreAttribute is null)
        {
            return JsonIgnoreCondition.Never;
        }

        return jsonIgnoreAttribute.GetAttributePropertyValue("Condition") switch
        {
            int condition => (JsonIgnoreCondition)condition,
            _ => JsonIgnoreCondition.Always
        };

        static bool IsJsonIgnoreAttribute(AttributeData attribute)
            =>
            attribute?.AttributeClass?.IsType("System.Text.Json.Serialization", "JsonIgnoreAttribute") is true;
    }

    private enum JsonIgnoreCondition
    {
        Never,

        Always,

        WhenWritingDefault,

        WhenWritingNull
    }
}