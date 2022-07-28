using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal static partial class EndpointAttributeHelper
{
    private const string AttributeNamespace = "GGroupp.Infra";

    private const string FullBodyInAttribute = "FullBodyInAttribute";

    private const string FullBodyOutAttribute = "FullBodyOutAttribute";

    private const string JsonBodyInAttribute = "JsonBodyInAttribute";

    private const string JsonBodyOutAttribute = "JsonBodyOutAttribute";

    private const string ClaimInAttribute = "ClaimInAttribute";

    private const string HeaderInAttribute = "HeaderInAttribute";

    private const string HeaderOutAttribute = "HeaderOutAttribute";

    private const string QueryInAttribute = "QueryInAttribute";

    private const string RouteInAttribute = "RouteInAttribute";

    private const string EndpointTagAttribute = "EndpointTagAttribute";

    private const string SuccessAttribute = "SuccessAttribute";

    private static bool IsEndpointAttribute(AttributeData attributeData, string attributeTypeName)
        =>
        attributeData.AttributeClass?.IsType(AttributeNamespace, attributeTypeName) is true;
}