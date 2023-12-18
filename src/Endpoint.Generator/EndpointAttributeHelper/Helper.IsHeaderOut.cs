using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsHeaderOutAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, HeaderOutAttribute);
}