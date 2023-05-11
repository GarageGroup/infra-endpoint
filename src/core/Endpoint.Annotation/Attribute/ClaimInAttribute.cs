using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ClaimInAttribute : Attribute
{
    public ClaimInAttribute([AllowNull] string claimType = null)
        =>
        ClaimType = string.IsNullOrEmpty(claimType) ? null : claimType;

    public string? ClaimType { get; }
}