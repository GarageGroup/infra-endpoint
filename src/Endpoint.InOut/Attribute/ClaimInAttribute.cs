using System;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ClaimInAttribute : Attribute
{
    public ClaimInAttribute()
        =>
        ClaimType = null;

    public ClaimInAttribute(string claimType)
        =>
        ClaimType = string.IsNullOrEmpty(claimType) ? null : claimType;

    public string? ClaimType { get; }
}