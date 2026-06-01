using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class EndpointSetAttribute : Attribute;