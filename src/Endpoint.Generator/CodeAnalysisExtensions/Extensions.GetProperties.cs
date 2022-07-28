using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static IReadOnlyCollection<IPropertySymbol> GetJsonProperties(this ITypeSymbol typeSymbol)
        =>
        typeSymbol.GetMembers().OfType<IPropertySymbol>().Where(IsNotIgnored).ToArray();

    private static bool IsNotIgnored(IPropertySymbol propertySymbol)
    {
        return propertySymbol.GetAttributes().Any(IsJsonIgnoreAttribute) is false;

        static bool IsJsonIgnoreAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsType("System.Text.Json.Serialization", "JsonIgnoreAttribute") is true;
    }
}