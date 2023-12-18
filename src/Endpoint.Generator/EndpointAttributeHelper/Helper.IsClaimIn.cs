using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsClaimInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, ClaimInAttribute);
}