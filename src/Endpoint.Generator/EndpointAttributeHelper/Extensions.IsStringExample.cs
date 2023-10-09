using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsStringExampleAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, StringExampleAttribute);
}