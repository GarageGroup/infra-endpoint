using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointAttributeHelper
{
    internal static bool IsRouteInAttribute(AttributeData attributeData)
        =>
        InnerIsEndpointAttribute(attributeData, RouteInAttribute);
}