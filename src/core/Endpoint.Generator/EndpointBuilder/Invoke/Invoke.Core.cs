using System;
using System.Collections.Generic;
using System.Linq;
using GGroupp;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

using static EndpointAttributeHelper;

partial class EndpointBuilder
{
    private static string[] GetParserSystemTypes()
        =>
        new[]
        {
            nameof(Boolean), nameof(Byte), "DateOnly", nameof(DateTime), nameof(DateTimeOffset), nameof(Decimal), nameof(Double),
            nameof(Guid), nameof(Int16), nameof(Int32), nameof(Int64), nameof(Single), "TimeOnly", nameof(TimeSpan)
        };

    private static string[] GetJsonDeserializerSystemTypes()
        =>
        new[]
        {
            nameof(Boolean), nameof(Byte), "DateOnly", nameof(DateTime), nameof(DateTimeOffset), nameof(Decimal), nameof(Double),
            nameof(Guid), nameof(Int16), nameof(Int32), nameof(Int64), nameof(Single), "TimeOnly", nameof(TimeSpan)
        };

    private static string[] GetJsonNumberSystemTypes()
        =>
        new[]
        {
            nameof(Byte), nameof(Decimal), nameof(Double), nameof(Int16), nameof(Int32), nameof(Int64), nameof(Single)
        };

    private static string GetMethodFuncName(this EndpointTypeDescription type)
        =>
        string.IsNullOrEmpty(type.MethodFuncName) ? "InvokeAsync" : type.MethodFuncName!;

    private static string GetRequestTypeName(this EndpointTypeDescription type)
        =>
        string.IsNullOrEmpty(type.RequestType?.Name) ? "HttpIn" : type.RequestType?.Name!;

    private static string GetResponseTypeName(this EndpointTypeDescription type)
        =>
        string.IsNullOrEmpty(type.ResponseType?.Name) ? "HttpOut" : type.ResponseType?.Name!;

    private static string GetFailureCodeTypeName(this EndpointTypeDescription type)
        =>
        string.IsNullOrEmpty(type.FailureCodeType?.Name) ? "Unit" : type.FailureCodeType?.Name!;

    private static IReadOnlyCollection<KeyValuePair<string, IPropertySymbol>> GetHeaderOutProperties(this EndpointTypeDescription type)
    {
        return InnerGetHeaderOutProperties().ToArray();

        IEnumerable<KeyValuePair<string, IPropertySymbol>> InnerGetHeaderOutProperties()
        {
            var properties = type?.ResponseType?.GetMembers().OfType<IPropertySymbol>().Where(IsPublic).Where(IsReadable);

            foreach (var property in properties ?? Enumerable.Empty<IPropertySymbol>())
            {
                var headerAttribute = property.GetAttributes().FirstOrDefault(IsHeaderOutAttribute);
                if (headerAttribute is null)
                {
                    continue;
                }

                var headerName = headerAttribute.GetAttributeValue(0, "HeaderName")?.ToString();
                yield return new(
                    key: string.IsNullOrEmpty(headerName) ? property.Name : headerName!,
                    value: property);
            }
        }

        static bool IsPublic(IPropertySymbol propertySymbol)
            =>
            propertySymbol.DeclaredAccessibility is Accessibility.Public;

        static bool IsReadable(IPropertySymbol propertySymbol)
            =>
            propertySymbol.GetMethod is not null;
    }

    private static IReadOnlyDictionary<string, string> GetSuccessStatusCodeNames(this ITypeSymbol successStatusCodeType)
    {
        var result = new Dictionary<string, string>();

        foreach (var successStatusCodeField in successStatusCodeType.GetEnumFields())
        {
            var successAttribute = successStatusCodeField.GetAttributes().FirstOrDefault(IsSuccessAttribute);
            if (successAttribute is null)
            {
                continue;
            }

            var successStatusCode = successAttribute.GetAttributeValue(0, "StatusCode")?.ToString();
            if (string.IsNullOrEmpty(successStatusCode))
            {
                continue;
            }

            result[successStatusCodeField.Name] = successStatusCode!;
        }

        return result;
    }

    private static string? GetSuccessStatusCodeValue(this EndpointTypeDescription type)
        =>
        type.ResponseType?.GetAttributes().FirstOrDefault(IsSuccessAttribute)?.GetAttributeValue(0, "StatusCode")?.ToString();

    private static string GetRequestFunctionValue(this IParameterSymbol parameter)
    {
        if (parameter.GetAttributes().FirstOrDefault(IsRouteInAttribute) is AttributeData routeInAttribute)
        {
            var name = routeInAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
            return $"GetRouteValue({GetParameterName(name).ToStringValueOrEmpty()})";
        }
        
        if (parameter.GetAttributes().FirstOrDefault(IsQueryInAttribute) is AttributeData queryInAttribute)
        {
            var name = queryInAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
            return $"GetQueryParameterValue({GetParameterName(name).ToStringValueOrEmpty()})";
        }

        if (parameter.GetAttributes().FirstOrDefault(IsHeaderInAttribute) is AttributeData headerInAttribute)
        {
            var name = headerInAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
            return $"GetHeaderValue({GetParameterName(name).ToStringValueOrEmpty()})";
        }

        if (parameter.GetAttributes().FirstOrDefault(IsClaimInAttribute) is AttributeData claimInAttribute)
        {
            var name = claimInAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
            return $"GetClaimValue({GetParameterName(name).ToStringValueOrEmpty()})";
        }

        return $"GetQueryParameterValue({parameter.Name.ToStringValueOrEmpty()})";

        string GetParameterName(string? fromAttribute)
            =>
            string.IsNullOrEmpty(fromAttribute) ? parameter.Name : fromAttribute!;
    }
}