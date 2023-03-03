using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsJsonBodyOutAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, JsonBodyOutAttribute);
}