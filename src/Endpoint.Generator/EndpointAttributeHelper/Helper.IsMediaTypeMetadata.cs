using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsMediaTypeMetadataAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, MediaTypeMetadataAttribute);
}