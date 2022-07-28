using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsHeaderInAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, HeaderInAttribute);
}