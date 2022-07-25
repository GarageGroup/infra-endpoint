using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal static partial class EndpointAttributeHelper
{
    private const string AttributeNamespace = "GGroupp.Infra";

    private const string BodyInAttribute = "BodyInAttribute";

    private const string ClaimInAttribute = "ClaimInAttribute";

    private const string HeaderInAttribute = "HeaderInAttribute";

    private const string QueryInAttribute = "QueryInAttribute";

    private const string RouteInAttribute = "RouteInAttribute";

    private const string EndpointTagAttribute = "EndpointTagAttribute";

    private static bool IsEndpointAttribute(AttributeData attributeData, string attributeTypeName)
        =>
        attributeData.AttributeClass?.IsType(AttributeNamespace, attributeTypeName) is true;
}