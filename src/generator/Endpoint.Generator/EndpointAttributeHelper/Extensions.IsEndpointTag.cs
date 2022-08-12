using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsEndpointTagAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, EndpointTagAttribute);
}