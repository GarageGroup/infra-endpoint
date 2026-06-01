using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ClaimInAttribute([AllowNull] string claimType = null) : Attribute
{
    public string? ClaimType { get; } = string.IsNullOrEmpty(claimType) ? null : claimType;
}