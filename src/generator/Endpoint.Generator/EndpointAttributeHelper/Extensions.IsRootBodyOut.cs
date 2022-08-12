using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsRootBodyOutAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, RootBodyOutAttribute);
}