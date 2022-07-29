using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static IReadOnlyCollection<IPropertySymbol> GetJsonProperties(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().OfType<IPropertySymbol>().Where(IsPublic).Where(IsNotIgnored).ToArray();

        static bool IsPublic(IPropertySymbol propertySymbol)
            =>
            propertySymbol.DeclaredAccessibility is Accessibility.Public;

        static bool IsNotIgnored(IPropertySymbol propertySymbol)
            =>
            propertySymbol.GetAttributes().Any(IsJsonIgnoreAttribute) is false;

        static bool IsJsonIgnoreAttribute(AttributeData attributeData)
            =>
            attributeData?.AttributeClass?.IsType("System.Text.Json.Serialization", "JsonIgnoreAttribute") is true;
    }
}