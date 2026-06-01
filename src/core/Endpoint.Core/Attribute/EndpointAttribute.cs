using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class EndpointAttribute : Attribute
{
    private const string ObsoleteMessage
        =
        $"{nameof(EndpointAttribute)}(EndpointMethod method, string route) is obsolete and will be removed in a future version. " +
        $"Use {nameof(EndpointAttribute)}(string operationId, EndpointMethod method, string route).";

    [Obsolete(ObsoleteMessage)]
    public EndpointAttribute(EndpointMethod method, string route)
        : this(
            operationId: string.Empty,
            method: method,
            route: route)
    {
    }

    public EndpointAttribute(string operationId, EndpointMethod method, string route)
    {
        OperationId = operationId ?? string.Empty;
        Method = method;
        Route = route ?? string.Empty;
    }

    public string OperationId { get; }

    public EndpointMethod Method { get; }

    public string Route { get; }

    public string? Summary { get; set; }

    public string? Description { get; set; }
}
