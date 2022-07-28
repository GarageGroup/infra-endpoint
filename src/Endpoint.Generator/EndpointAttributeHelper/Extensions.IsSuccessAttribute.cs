using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsSuccessAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, SuccessAttribute);
}