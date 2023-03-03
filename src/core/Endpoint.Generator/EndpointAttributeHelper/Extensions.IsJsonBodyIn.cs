using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsJsonBodyInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, JsonBodyInAttribute);
}