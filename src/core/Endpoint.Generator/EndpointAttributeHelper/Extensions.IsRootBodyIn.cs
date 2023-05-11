using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsRootBodyInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, RootBodyInAttribute);
}