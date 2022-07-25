using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsBodyInAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, BodyInAttribute);
}