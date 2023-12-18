using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsJsonBodyInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, JsonBodyInAttribute);
}