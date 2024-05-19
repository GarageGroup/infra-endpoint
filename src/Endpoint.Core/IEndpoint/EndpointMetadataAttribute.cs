using System;

namespace GarageGroup.Infra.Endpoint;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EndpointMetadataAttribute(string method, string route) : Attribute
{
    public string Method { get; } = method ?? string.Empty;

    public string Route { get; } = route ?? string.Empty;
}