using System;

namespace GarageGroup.Infra.Endpoint;

[AttributeUsage(AttributeTargets.Class)]
[Obsolete(ObsoleteMessage)]
public sealed class EndpointMetadataAttribute(string method, string route) : Attribute
{
    private const string ObsoleteMessage
        =
        $"{nameof(EndpointMetadataAttribute)} is obsolete and will be removed in a future version. " +
        $"Use {nameof(EndpointOperationMetadataAttribute)} instead.";

    public string Method { get; } = method ?? string.Empty;

    public string Route { get; } = route ?? string.Empty;
}
