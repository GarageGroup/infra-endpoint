using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsEndpointTagAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, EndpointTagAttribute);
}