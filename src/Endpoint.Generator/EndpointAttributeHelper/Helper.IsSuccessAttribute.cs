using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsSuccessAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, SuccessAttribute);
}