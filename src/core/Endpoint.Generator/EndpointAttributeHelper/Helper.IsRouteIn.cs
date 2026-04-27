using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsRouteInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, RouteInAttribute);
}