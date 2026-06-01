using System;

namespace GarageGroup.Infra.Endpoint;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class EndpointOperationMetadataAttribute(string operationId, string method, string route) : Attribute
{
    public string OperationId { get; } = operationId ?? string.Empty;

    public string Method { get; } = method ?? string.Empty;

    public string Route { get; } = route ?? string.Empty;
}
