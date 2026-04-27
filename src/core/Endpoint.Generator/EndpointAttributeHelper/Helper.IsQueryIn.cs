using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsQueryInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, QueryInAttribute);
}