using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsHeaderInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, HeaderInAttribute);
}