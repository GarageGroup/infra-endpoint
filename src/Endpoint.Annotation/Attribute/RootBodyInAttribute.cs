using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class RootBodyInAttribute([AllowNull] string contentType = RootBodyInAttribute.DefaultContentType) : Attribute
{
    private const string DefaultContentType = MediaTypeNames.Application.Json;

    public string ContentType { get; } = string.IsNullOrEmpty(contentType) ? DefaultContentType : contentType;
}