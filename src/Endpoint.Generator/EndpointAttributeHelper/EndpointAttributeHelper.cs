using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

internal static partial class EndpointAttributeHelper
{
    private const string AttributeNamespace = "GarageGroup.Infra";

    private const string RootBodyInAttribute = "RootBodyInAttribute";

    private const string RootBodyOutAttribute = "RootBodyOutAttribute";

    private const string JsonBodyInAttribute = "JsonBodyInAttribute";

    private const string JsonBodyOutAttribute = "JsonBodyOutAttribute";

    private const string ClaimInAttribute = "ClaimInAttribute";

    private const string HeaderInAttribute = "HeaderInAttribute";

    private const string HeaderOutAttribute = "HeaderOutAttribute";

    private const string QueryInAttribute = "QueryInAttribute";

    private const string RouteInAttribute = "RouteInAttribute";

    private const string EndpointTagAttribute = "EndpointTagAttribute";

    private const string SuccessAttribute = "SuccessAttribute";

    private const string ExampleAttribute = "ExampleAttributeBase";

    private const string SwaggerDescriptionAttribute = "SwaggerDescriptionAttribute";

    private static bool InnerIsEndpointAttribute(AttributeData attributeData, string attributeTypeName)
        =>
        attributeData.AttributeClass?.IsType(AttributeNamespace, attributeTypeName) is true;
}