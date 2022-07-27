using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsFullBodyInAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, FullBodyInAttribute);
}