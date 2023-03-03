using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsHeaderOutAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, HeaderOutAttribute);
}