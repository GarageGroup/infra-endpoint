using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsSwaggerDescriptionAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, SwaggerDescriptionAttribute);
}