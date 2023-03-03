using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsRootBodyInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, RootBodyInAttribute);
}