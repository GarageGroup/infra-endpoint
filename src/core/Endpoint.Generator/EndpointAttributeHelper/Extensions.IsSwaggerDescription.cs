using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsSwaggerDescriptionAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, SwaggerDescriptionAttribute);
}