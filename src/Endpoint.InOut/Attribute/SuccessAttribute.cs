using System;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
public sealed class SuccessAttribute : Attribute
{
    public SuccessAttribute()
        =>
        StatusCode = SuccessStatusCode.Ok;

    public SuccessAttribute(SuccessStatusCode statusCode)
        =>
        StatusCode = statusCode;

    public SuccessStatusCode StatusCode { get; }
}