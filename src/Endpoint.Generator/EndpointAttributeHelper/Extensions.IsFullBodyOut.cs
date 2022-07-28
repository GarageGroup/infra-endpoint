using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsFullBodyOutAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, FullBodyOutAttribute);
}