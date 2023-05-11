using System;

namespace GarageGroup.Infra.Endpoint;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EndpointMetadataAttribute : Attribute
{
    public EndpointMetadataAttribute(string method, string route)
    {
        Method = method ?? string.Empty;
        Route = route ?? string.Empty;
    }

    public string Method { get; }

    public string Route { get; }
}