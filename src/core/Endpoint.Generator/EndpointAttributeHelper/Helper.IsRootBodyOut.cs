using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsRootBodyOutAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, RootBodyOutAttribute);
}