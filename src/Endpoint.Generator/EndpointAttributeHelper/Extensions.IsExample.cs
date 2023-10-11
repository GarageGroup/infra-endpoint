using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsExampleAttribute(AttributeData attributeData)
        =>
        attributeData.AttributeClass?.BaseType?.IsType(AttributeNamespace, ExampleAttribute) is true;
}