using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsRootBodyOutAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, RootBodyOutAttribute);
}