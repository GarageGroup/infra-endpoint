using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsJsonBodyOutAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, JsonBodyOutAttribute);
}