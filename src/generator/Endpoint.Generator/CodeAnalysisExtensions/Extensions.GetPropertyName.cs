using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static string GetJsonPropertyName(this IPropertySymbol propertySymbol)
    {
        var jsonPropertyNameAttribute = propertySymbol.GetAttributes().FirstOrDefault(IsJsonPropertyNameAttribute);
        if (jsonPropertyNameAttribute is not null)
        {
            var name = jsonPropertyNameAttribute.GetAttributeValue(0)?.ToString();
            if (string.IsNullOrEmpty(name) is false)
            {
                return name!;
            }
        }

        return propertySymbol.Name.WithCamelCase();

        static bool IsJsonPropertyNameAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsType("System.Text.Json.Serialization", "JsonPropertyNameAttribute") is true;
    }
}