using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsFormBodyInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, FormBodyInAttribute);
}