using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsEndpointTagAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, EndpointTagAttribute);
}