using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsStringExampleAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, StringExampleAttribute);
}