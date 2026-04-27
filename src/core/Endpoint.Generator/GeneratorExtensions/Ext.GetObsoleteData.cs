using System.Linq;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class SourceGeneratorExtensions
{
    internal static ObsoleteData? GetObsoleteData(this ISymbol symbol)
    {
        var obsoleteAttributeData = symbol.GetAttributes().FirstOrDefault(IsObsoleteAttribute);
        if (obsoleteAttributeData is null)
        {
            return null;
        }

        return new(
            message: obsoleteAttributeData.GetAttributeValue(0)?.ToString(),
            isError: obsoleteAttributeData.GetAttributeValue(1) as bool?,
            diagnosticId: obsoleteAttributeData.GetAttributePropertyValue("DiagnosticId")?.ToString(),
            urlFormat: obsoleteAttributeData.GetAttributePropertyValue("UrlFormat")?.ToString());

        static bool IsObsoleteAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsSystemType("ObsoleteAttribute") is true;
    }
}