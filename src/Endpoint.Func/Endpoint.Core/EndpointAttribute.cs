using System;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EndpointAttribute : Attribute
{
    public EndpointAttribute(EndpointMethod method, string route)
    {
        Method = method;
        Route = route ?? string.Empty;
    }

    public EndpointMethod Method { get; }

    public string Route { get; }

    public string? Summary { get; set; }

    public string? Description { get; set; }
}