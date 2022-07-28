using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static object? GetAttributePropertyValue(this AttributeData attributeData, string propertyName)
    {
        return attributeData.NamedArguments.FirstOrDefault(IsNameMatched).Value.Value;

        bool IsNameMatched(KeyValuePair<string, TypedConstant> pair)
            =>
            string.Equals(pair.Key, propertyName, StringComparison.InvariantCulture);
    }

    internal static object? GetAttributeValue(this AttributeData attributeData, int constructorOrder, string? propertyName = null)
    {
        if (attributeData.ConstructorArguments.Length > constructorOrder)
        {
            return attributeData.ConstructorArguments[constructorOrder].Value;
        }

        if (string.IsNullOrEmpty(propertyName))
        {
            return null;
        }

        return attributeData.GetAttributePropertyValue(propertyName!);
    }
}