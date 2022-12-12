using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsQueryInAttribute(AttributeData attributeData)
        =>
        IsEndpointAttribute(attributeData, QueryInAttribute);
}