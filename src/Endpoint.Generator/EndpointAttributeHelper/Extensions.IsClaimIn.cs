using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsClaimInAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, ClaimInAttribute);
}