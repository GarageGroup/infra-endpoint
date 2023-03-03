using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsClaimInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, ClaimInAttribute);
}