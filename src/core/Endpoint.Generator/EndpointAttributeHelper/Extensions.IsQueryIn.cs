using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsQueryInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, QueryInAttribute);
}