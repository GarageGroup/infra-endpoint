using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsHeaderInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, HeaderInAttribute);
}