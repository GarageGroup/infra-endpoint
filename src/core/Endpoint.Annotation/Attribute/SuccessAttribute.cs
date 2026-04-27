using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
public sealed class SuccessAttribute(SuccessStatusCode statusCode = SuccessStatusCode.Ok) : Attribute
{
    public SuccessStatusCode StatusCode { get; } = statusCode;

    public string? Description { get; set; }
}