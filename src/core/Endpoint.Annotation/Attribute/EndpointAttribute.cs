using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class EndpointAttribute(EndpointMethod method, string route) : Attribute
{
    public EndpointMethod Method { get; } = method;

    public string Route { get; } = route ?? string.Empty;

    public string? Summary { get; set; }

    public string? Description { get; set; }
}