using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsHeaderOutAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, HeaderOutAttribute);
}